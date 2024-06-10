using MelbergFramework.Infrastructure.Redis;
using Microsoft.Extensions.Options;

namespace Infrastructure.Redis.Contexts;
public class PlaneCacheContext : RedisContext
{
    public PlaneCacheContext(
            IOptions<RedisConnectionOptions<PlaneCacheContext>> options,
            IConnector connector) : base(options.Value, connector) { }
}
