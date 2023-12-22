using Domain;

namespace Infrastructure.RepositoryCore;

public interface IPlaneCacheRepository
{
    Task InsertNodePlaneFrameAsync(PlaneFrame frame);
    Task InsertCompiledPlaneFrameAsync(PlaneFrame frame);
    IAsyncEnumerable<PlaneFrame> CollectPlaneStatesAsync(long timestamp);
    Task<PlaneFrame> GetCompiledPlaneFrameAsync(long timestamp);
}