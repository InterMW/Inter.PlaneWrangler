using Common;
using Device.Domain;
using Device.GrpcClient;
using Domain;
using Infrastructure.RepositoryCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DomainService;
public interface IPlaneIngestDomainService
{
    Task IngestPlaneFrameAsync(PlaneFrame frame);
}

public class PlaneIngestDomainService(
        IDeviceGrpcClient client,
        IPlaneCacheRepository _planeCacheRepository,
        IPlaneMetadataRepository _planeMetadataRepository,
        IOptions<MetadataOptions> metadataOptions,
        ILogger<PlaneIngestDomainService> logger
        ) : IPlaneIngestDomainService
{
    private readonly int _metadataScale = metadataOptions.Value.Scale;

    public async Task IngestPlaneFrameAsync(PlaneFrame frame)
    {
        if (frame.Planes.Count() == 0)
        {
            return;
        }

        DeviceModel device = new();

        try
        {
            device = await client.GetDeviceAsync(frame.Source);
        }
        catch (Device.Common.DeviceNotFoundException)
        {
            logger.LogInformation("Did not process message from {device} because it was not registered", frame.Source);
            return;
        }

        await _planeCacheRepository.InsertNodePlaneFrameAsync(frame);


        var metadata = new PlaneFrameMetadata()
        {
            Total = frame.Planes.Count(),
            Detailed = frame.Planes.Where(DetailedFilter)
                            .Count(),
            Antenna = frame.Antenna,
            Hostname = frame.Source,
            Timestamp = DateTime.UnixEpoch.AddSeconds(frame.Now)
        };

        if (device.Latitude != 0 && device.Longitude != 0)
        {
            var cleanedPlanes = frame.Planes.Where(DetailedFilter).Where(_ => Distance(_.Latitude!.Value, _.Longitude!.Value, device.Latitude!, device.Longitude!) < 6);
            metadata = AnnotateMetadataWithDistanceDetail(device, metadata, cleanedPlanes);
        }

        await _planeMetadataRepository.LogPlaneMetadata(metadata);
    }

    private PlaneFrameMetadata AnnotateMetadataWithDistanceDetail(
            DeviceModel device,
            PlaneFrameMetadata frame,
            IEnumerable<TimeAnotatedPlane> planes)
    {
        if (planes.Count() == 0)
        {
            return frame;
        }

        frame.Detailed = planes.Count();

        float originLatitude = device.Latitude;
        float originLongitude = device.Longitude;

        float maxDistance = 0;
        float totalDistance = 0;
        float rssiSum = 0;


        foreach (var plane in planes)
        {
            var planeLat = plane.Latitude!.Value;
            var planeLon = plane.Longitude!.Value;


            rssiSum += plane.Rssi!.Value;
            var distance = Distance(originLatitude, originLongitude, plane.Latitude!.Value, plane.Longitude!.Value);

            maxDistance = float.Max(distance, maxDistance);
            totalDistance += distance;
        }

        float averageDistance = totalDistance / planes.Count();

        frame.MaxDistance = maxDistance * _metadataScale;
        frame.AverageDistance = averageDistance * _metadataScale;

        return frame;
    }
    private static float Distance(float lat, float lon, float lat2, float lon2) =>

        (float)Math.Sqrt(Math.Pow(lat - lat2, 2) + Math.Pow(lon - lon2, 2));

    private bool DetailedFilter(Plane plane) =>
        plane.Latitude.HasValue && plane.Longitude.HasValue;
}
