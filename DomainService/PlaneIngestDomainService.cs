using Domain;
using Infrastructure.RepositoryCore;

namespace DomainService;
public interface IPlaneIngestDomainService
{
    Task IngestPlaneFrameAsync(PlaneFrame frame);
}

public class PlaneIngestDomainService : IPlaneIngestDomainService
{
    private readonly IPlaneCacheRepository _planeCacheRepository;
    private readonly IPlaneMetadataRepository _planeMetadataRepository;

    public PlaneIngestDomainService(
        IPlaneCacheRepository planeCacheRepository,
        IPlaneMetadataRepository planeMetadataRepository)
    {
        _planeCacheRepository = planeCacheRepository;
        _planeMetadataRepository = planeMetadataRepository;
    }

    public Task IngestPlaneFrameAsync(PlaneFrame frame) =>
        Task.WhenAll(
            _planeCacheRepository.InsertNodePlaneFrameAsync(frame),
            RecordMetadataAsync(frame));

    private Task RecordMetadataAsync(PlaneFrame frame) =>
        _planeMetadataRepository.LogPlaneMetadata(
            new PlaneFrameMetadata()
            {
                Total = frame.Planes.Count(),
                Detailed = frame
                            .Planes
                            .Where(DetailedFilter)
                            .Count(),
                Antenna = frame.Antenna,
                Hostname = frame.Source,
                Timestamp = DateTime.UnixEpoch.AddSeconds(frame.Now)
            }
        );

    private bool DetailedFilter(Plane plane) =>
        plane.Latitude.HasValue && plane.Longitude.HasValue;
}