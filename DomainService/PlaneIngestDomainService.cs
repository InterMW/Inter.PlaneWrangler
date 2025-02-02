using Device.Domain;
using Device.GrpcClient;
using Domain;
using Infrastructure.RepositoryCore;

namespace DomainService;
public interface IPlaneIngestDomainService
{
    Task IngestPlaneFrameAsync(PlaneFrame frame);
}

public class PlaneIngestDomainService(
        IDeviceGrpcClient client,
        IPlaneCacheRepository _planeCacheRepository,
        IPlaneMetadataRepository _planeMetadataRepository
        ) : IPlaneIngestDomainService
{

    public async Task IngestPlaneFrameAsync(PlaneFrame frame)
    {
        DeviceModel device = new();

        try
        {
            device = await client.GetDeviceAsync(frame.Source);
        }
        catch (Device.Common.DeviceNotFoundException)
        {
            Console.WriteLine("a");
            return;
        }

        await _planeCacheRepository.InsertNodePlaneFrameAsync(frame);

        float originLatitude = device.Latitude;
        float originLongitude = device.Longitude;

        float maxDistance = 0;
        float totalDistance = 0;
        float rssiSum = 0;

        var planes = frame.Planes;

        foreach(var plane in planes.Where(_ => _.Latitude is not null && _.Longitude is not null))
        {
            var planeLat = plane.Latitude!.Value;
            var planeLon = plane.Longitude!.Value;

            rssiSum += plane.Rssi!.Value;
            var distance = Distance(originLatitude, originLongitude, plane.Latitude!.Value, plane.Longitude!.Value);
            maxDistance = float.Max(distance, maxDistance);
            totalDistance += distance;
        }

        float averageDistance = totalDistance / planes.Count();

        Console.WriteLine($"average distance = {averageDistance}");
        await _planeMetadataRepository.LogPlaneMetadata(
            new PlaneFrameMetadata()
            {
                Total = frame.Planes.Count(),
                Detailed = frame
                            .Planes
                            .Where(DetailedFilter)
                            .Count(),
                MaxDistance = maxDistance,
                AverageDistance = averageDistance, 
                Antenna = frame.Antenna,
                Hostname = frame.Source,
                Timestamp = DateTime.UnixEpoch.AddSeconds(frame.Now)
            }
        );
    }
    
    private float Distance(float lat, float lon, float lat2, float lon2) =>

        (float)Math.Sqrt(Math.Pow(lat - lat2, 2) + Math.Pow(lon - lon2,2));

    private bool DetailedFilter(Plane plane) =>
        plane.Latitude.HasValue && plane.Longitude.HasValue;
}
