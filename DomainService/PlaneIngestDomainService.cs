using Device.Domain;
using Device.GrpcClient;
using Domain;
using Infrastructure.RepositoryCore;
using Microsoft.Extensions.Logging;

namespace DomainService;
public interface IPlaneIngestDomainService
{
    Task IngestPlaneFrameAsync(PlaneFrame frame);
}

public class PlaneIngestDomainService(
        IDeviceGrpcClient client,
        IPlaneCacheRepository _planeCacheRepository,
        IPlaneMetadataRepository _planeMetadataRepository,
        ILogger<PlaneIngestDomainService> logger
        ) : IPlaneIngestDomainService
{

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

        var planes = frame.Planes.Where(DetailedFilter);

        if (planes.Count() == 0)
        {
            return;
        }

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

        (float)Math.Sqrt(Math.Pow(lat - lat2, 2) + Math.Pow(lon - lon2, 2));

    private bool DetailedFilter(Plane plane) =>
        plane.Latitude.HasValue && plane.Longitude.HasValue;
}
