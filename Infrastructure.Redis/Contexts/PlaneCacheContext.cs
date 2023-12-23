using MelbergFramework.Core.Redis;
using MelbergFramework.Infrastructure.Redis;

namespace Infrastructure.Redis.Contexts;
public class PlaneCacheContext : RedisContext
{
    public PlaneCacheContext(IRedisConfigurationProvider provider) : base(provider) { }
}