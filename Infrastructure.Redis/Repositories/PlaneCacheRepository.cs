using System.Diagnostics;
using Domain;
using Infrastructure.Redis.Contexts;
using Infrastructure.Redis.Mappers;
using Infrastructure.RepositoryCore;
using MelbergFramework.Infrastructure.Redis.Repository;

namespace Infrastructure.Redis.Repositories;

public class PlaneCacheRepository : RedisRepository<PlaneCacheContext>, IPlaneCacheRepository
{
    private TimeSpan FrameLifespan => new TimeSpan(0,0,45);

    public PlaneCacheRepository(PlaneCacheContext context) : base(context) { }

    public Task InsertNodePlaneFrameAsync(PlaneFrame frame) =>
        DB.StringSetAsync(
            ToPreAggregateKey(frame),
            frame.ToModel().ToPayload(),
            FrameLifespan);

    public Task InsertCompiledPlaneFrameAsync(PlaneFrame frame) =>
        DB.StringSetAsync(
            ToCongregatedKey(frame),
            frame.ToModel().ToPayload(),
            FrameLifespan
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
            yield return (await DB.StringGetAsync(key)).ToDomain(sourceDefinition);
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
