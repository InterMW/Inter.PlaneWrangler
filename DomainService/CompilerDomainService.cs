using System.Collections.Immutable;
using System.Diagnostics;
using Common;
using Domain;
using Infrastructure.RepositoryCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DomainService;

public interface ICompilerDomainService
{
    Task CompilePlanesAsync(long time);
}

public class CompilerDomainService : ICompilerDomainService
{
    private readonly IPlaneCacheRepository _planeCacheRepository;
    private readonly IPlaneMetadataRepository _planeMetadataRepository;
    private readonly IPlaneFramePublisher _planeFramePublisher;
    private readonly long _timestampOffset;
    private readonly ILogger<CompilerDomainService> _logger;

    public CompilerDomainService(
        IPlaneCacheRepository planeCacheRepository,
        IPlaneMetadataRepository planeMetadataRepository,
        IPlaneFramePublisher planeFramePublisher,
        IOptions<TimingsOptions> timingsOptions,
        ILogger<CompilerDomainService> logger)

    {
        _planeCacheRepository = planeCacheRepository;
        _planeMetadataRepository = planeMetadataRepository;
        _planeFramePublisher = planeFramePublisher;
        _timestampOffset = timingsOptions.Value.CompilationOffsetSecs;
        _logger = logger;
    }

    public async Task CompilePlanesAsync(long timestamp)
    {
        var offsetTimestamp = timestamp - _timestampOffset; // look at some previous moment

        Console.Write($"Congregating {offsetTimestamp}");
        
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var totalState = await GetPlaneStatesAndCombine(offsetTimestamp);
        stopwatch.Stop();
        _logger.LogWarning($"It took totalstate {stopwatch.ElapsedMilliseconds}");


        var congregatedFrame = FilterPlanesIntoFrame(offsetTimestamp, totalState);
        
        await _planeCacheRepository.InsertCompiledPlaneFrameAsync(congregatedFrame);

        var metadata = CreateMetadataFromFrame(congregatedFrame);

       await _planeMetadataRepository.LogPlaneMetadata(metadata);

        _planeFramePublisher.PublishPlaneFrame(congregatedFrame);
        
        Console.WriteLine($" {metadata.Total}");
    }

    private async Task<IDictionary<string, TimeAnotatedPlane>> GetPlaneStatesAndCombine(long timeStamp) 
    {
        var totalState = new Dictionary<string, TimeAnotatedPlane>();

        await foreach(var planeFrame in _planeCacheRepository.CollectPlaneStatesAsync(timeStamp))
        {
            foreach(var plane in planeFrame.Planes)
            {
                SafeAdd(totalState, plane);
            }
        }
        
        return totalState;
    }

    private static PlaneFrame FilterPlanesIntoFrame(
        long timeStamp,
        IDictionary<string, 
        TimeAnotatedPlane> state) => new ()
        {
            Now = timeStamp,
            Planes = state.Values.Where(_ => HasValidPositionAndIsNew(_, timeStamp)).ToArray(),
            Source = "congregation",
            Antenna = "congregator"
        };

    private static bool HasValidPositionAndIsNew(TimeAnotatedPlane plane, long timeStamp) =>
            plane.Latitude != null && 
            plane.Longitude != null && 
            (( (long)(plane.PositionUpdated ?? 0) / 1000 + 30) > timeStamp);

    private static PlaneFrameMetadata CreateMetadataFromFrame(PlaneFrame frame) => new ()
    {
        Total = frame.Planes.Length,
        Detailed = frame.Planes.Length,
        Antenna = frame.Antenna,
        Hostname = frame.Source,
        Timestamp = DateTime.UnixEpoch.AddSeconds(frame.Now)
    };

    private static void SafeAdd(Dictionary<string,TimeAnotatedPlane> planeDictionary, TimeAnotatedPlane plane)
    {
        if(!planeDictionary.ContainsKey(plane.HexValue))
        {
            planeDictionary.Add(plane.HexValue,plane);
        }
        else
        {
            var currentRecord = planeDictionary.GetValueOrDefault(plane.HexValue);
            var updatePosition = CompareUpdated(true, false, currentRecord.PositionUpdated, plane.PositionUpdated);

            currentRecord.Altitude = CompareUpdated(currentRecord.Altitude, plane.Altitude, currentRecord.AltitudeUpdated, plane.AltitudeUpdated);
            currentRecord.AltitudeUpdated = BestUpdated(currentRecord.AltitudeUpdated,plane.AltitudeUpdated);
            currentRecord.Category = CompareUpdated(currentRecord.Category,plane.Category, currentRecord.CategoryUpdated, plane.CategoryUpdated);
            currentRecord.CategoryUpdated = BestUpdated(currentRecord.CategoryUpdated, plane.CategoryUpdated);
            currentRecord.Flight = CompareUpdated(currentRecord.Flight, plane.Flight, currentRecord.FlightUpdated, plane.FlightUpdated);
            currentRecord.FlightUpdated = BestUpdated(currentRecord.FlightUpdated, plane.FlightUpdated);
            currentRecord.Latitude = updatePosition ? currentRecord.Latitude : plane.Latitude;
            currentRecord.Longitude = updatePosition ? currentRecord.Longitude : plane.Longitude;
            currentRecord.Messages = "0";
            currentRecord.Nucp = updatePosition ? currentRecord.Nucp : plane.Nucp;
            currentRecord.PositionUpdated = BestUpdated(currentRecord.PositionUpdated, plane.PositionUpdated);
            currentRecord.Rssi = 0; //not important
            currentRecord.Speed = CompareUpdated(currentRecord.Speed, plane.Speed,currentRecord.SpeedUpdated, plane.SpeedUpdated);
            currentRecord.SpeedUpdated = BestUpdated(currentRecord.SpeedUpdated,plane.SpeedUpdated);
            currentRecord.Squawk = CompareUpdated(currentRecord.Squawk, plane.Squawk, currentRecord.SquawkUpdated, plane.SquawkUpdated);
            currentRecord.SquawkUpdated = BestUpdated(currentRecord.SpeedUpdated, plane.SquawkUpdated);
            currentRecord.Track = CompareUpdated(currentRecord.Track, plane.Track, currentRecord.TrackUpdated, plane.TrackUpdated);
            currentRecord.TrackUpdated = BestUpdated(currentRecord.TrackUpdated, plane.TrackUpdated);
            currentRecord.VerticleRate = CompareUpdated(currentRecord.VerticleRate, plane.VerticleRate, currentRecord.VerticleRateUpdated, plane.VerticleRateUpdated);
            currentRecord.VerticleRateUpdated = BestUpdated(currentRecord.VerticleRateUpdated, plane.VerticleRateUpdated);

            planeDictionary[plane.HexValue] = currentRecord;
        }
    }

    private static ulong BestUpdated(ulong? currentUpdated, ulong? selectedUpdated) => 
        (currentUpdated ?? 0) > (selectedUpdated ?? 0) ? 
        currentUpdated ?? 0 : selectedUpdated ?? 0;

    private static T CompareUpdated<T>(
        T currentValue,
        T selectedValue,
        ulong? currentUpdated,
        ulong? selectedUpdated) =>
        (currentUpdated ?? 0) > (selectedUpdated ?? 0) ? currentValue : selectedValue;
}