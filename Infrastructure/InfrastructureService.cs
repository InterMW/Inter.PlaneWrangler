using Inter.Domain;
using Inter.Infrastructure.Redis;

namespace Inter.Infrastructure.Service;

public interface IInfrastructureService
{
    Task InsertRawPlaneFrameAsync(PlaneFrame frame);
    Task InsertCombinedPlaneFrameAsync(PlaneFrame frame);
    IAsyncEnumerable<PlaneFrame> GetPreCongregatedPlaneFramesAsync(long timestamp);
    Task UploadPlaneFrameMetadataAsync(PlaneFrameMetadata metadata);
}

public class InfrastructureService : IInfrastructureService
{
    private readonly IPlaneCacheRepository _planeCacheRepository;
    public InfrastructureService(IPlaneCacheRepository planeCacheRepository)
    {
        _planeCacheRepository = planeCacheRepository;
    }

    public IAsyncEnumerable<PlaneFrame> GetPreCongregatedPlaneFramesAsync(long timestamp) =>
        _planeCacheRepository.GetPreCongregatedPlaneFramesAsync(timestamp);

    public Task InsertCombinedPlaneFrameAsync(PlaneFrame frame) => 
        _planeCacheRepository.InsertCongregatedPlaneFrameAsync(frame);

    public Task InsertRawPlaneFrameAsync(PlaneFrame frame) =>
        _planeCacheRepository.InsertPreCongregatedPlaneFrameAsync(frame);

    public Task UploadPlaneFrameMetadataAsync(PlaneFrameMetadata metadata)
    {
        throw new NotImplementedException();
    }
}