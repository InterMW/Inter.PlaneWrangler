using Domain;

namespace Infrastructure.RepositoryCore;

public interface IPlaneCacheRepository
{
    Task InsertNodePlaneFrameAsync(PlaneFrame frame);
    Task InsertCompiledPlaneFrameAsync(PlaneFrame frame);
    Task<PlaneFrame> GetCompiledPlaneFrameAsync(long timestamp);
    Task<IEnumerable<TimeAnotatedPlane>> CollectPlaneStatesAsync(long timestamp);

}