using Domain;
using Infrastructure.RepositoryCore;

namespace DomainService;

public interface IAccessDomainService
{
    Task<PlaneFrame> RetrieveRecentPlaneFrame(long time);
}

public class AccessDomainService : IAccessDomainService
{
    private readonly IPlaneCacheRepository _planeCacheRepository;

    public AccessDomainService(IPlaneCacheRepository planeCacheRepository)
    {
        _planeCacheRepository = planeCacheRepository;
    }
    
    public Task<PlaneFrame> RetrieveRecentPlaneFrame(long time) =>
        _planeCacheRepository.GetCompiledPlaneFrameAsync(time);
}