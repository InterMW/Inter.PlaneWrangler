using System.Diagnostics;
using Common;
using Domain;
using Infrastructure.Redis.Contexts;
using Infrastructure.Redis.Mappers;
using Infrastructure.RepositoryCore;
using MelbergFramework.Infrastructure.Redis.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Redis.Repositories;

public class PlaneCacheRepository : RedisRepository<PlaneCacheContext>, IPlaneCacheRepository
{
    private readonly TimeSpan _frameLifespan;
    private readonly ILogger<PlaneCacheRepository> _logger;

    public PlaneCacheRepository(PlaneCacheContext context, IOptions<TimingsOptions> options, ILogger<PlaneCacheRepository> logger) : base(context) 
    {
        _frameLifespan = new TimeSpan(0,0,options.Value.PlaneDocLifetimesSecs);
        _logger = logger;
    }

    public async Task InsertNodePlaneFrameAsync(PlaneFrame frame)
    {
        var stopwatch = new Stopwatch();
        var model = frame.ToModel();
        var payload = model.ToPayload();
        var key = ToPreAggregateKey(frame);
        var life = _frameLifespan;
        stopwatch.Restart();
        DB.StringSetAsync(
            key,
            payload,
            life).Wait();
        stopwatch.Stop();
        _logger.LogInformation($"{stopwatch.ElapsedMilliseconds}, {payload.Length},");
    }
    
    public Task InsertCompiledPlaneFrameAsync(PlaneFrame frame) =>
        DB.StringSetAsync(
            ToCongregatedKey(frame),
            frame.ToModel().ToPayload(),
            _frameLifespan
        );
    
    public async IAsyncEnumerable<PlaneFrame> CollectPlaneStatesAsync(long timestamp)
    {
        await foreach(var key in Server.KeysAsync(pattern:ToPreAggregateKey("*","*",timestamp)))
        {
            var keySections = key.ToString().Split("_");
            var sourceDefinition = new PlaneSourceDefintion()
            {
                Node = keySections[2],
                Antenna = keySections[3]
            };
            var result = DB.StringGetAsync(key);
            result.Wait();
            yield return result.Result.ToDomain(sourceDefinition);
        }
        yield break;
    }

    public async Task<PlaneFrame> GetCompiledPlaneFrameAsync(long timestamp) 
    {
        var payload = await DB.StringGetAsync(ToCongregatedKey(timestamp));
        var sourceDefinition = new PlaneSourceDefintion()
        {
            Node = "aggregate",
            Antenna = "aggregate"
        };
        var planes = payload.ToDomain(sourceDefinition);
        planes.Now = timestamp;
        return planes; 
    }

    private string ToPreAggregateKey(PlaneFrame frame) => 
        ToPreAggregateKey(frame.Source, frame.Antenna, frame.Now);
    private string ToPreAggregateKey(string source, string antenna, long time) => 
        $"wrangle_precongregate_{source}_{antenna}_{time}";
    private string ToCongregatedKey(PlaneFrame frame) => ToCongregatedKey(frame.Now);
    private string ToCongregatedKey(long time) => $"wrangle_plane_congregated_{time}";
}
