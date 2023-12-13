using Inter.Domain;
using Inter.Infrastructure.Redis.Mappers;
using MelbergFramework.Infrastructure.Redis.Repository;

namespace Inter.Infrastructure.Redis;
public interface IPlaneCacheRepository
{
    Task InsertPreCongregatedPlaneFrameAsync(PlaneFrame frame); 
    Task InsertCongregatedPlaneFrameAsync(PlaneFrame frame);
    IAsyncEnumerable<PlaneFrame> GetPreCongregatedPlaneFramesAsync(long timestamp);
}

public class PlaneCacheRepository : RedisRepository<PlaneCacheContext>, IPlaneCacheRepository
{ 
    private TimeSpan FrameLifespan => new TimeSpan(0,0,45);
    public PlaneCacheRepository(PlaneCacheContext context) : base(context) { }

    public Task InsertPreCongregatedPlaneFrameAsync(PlaneFrame planeFrame) =>
        DB.StringSetAsync(
            ToPreAggregateKey(planeFrame),
            planeFrame.ToModel().ToPayload(),
            FrameLifespan);
    public Task InsertCongregatedPlaneFrameAsync(PlaneFrame frame) =>
        DB.StringSetAsync(
            ToCongregatedKey(frame),
            frame.ToModel().ToPayload(),
            FrameLifespan);
    
    public async IAsyncEnumerable<PlaneFrame> GetPreCongregatedPlaneFramesAsync(long timestamp)
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
    public string ToCongregatedKey( PlaneFrame frame) =>
        $"m_plane_congregated_{frame.Now}";
    
    private string ToPreAggregateKey( PlaneFrame frame) =>
        ToPreAggregateKey(frame.Source, frame.Antenna, frame.Now);
    public string ToPreAggregateKey( string node, string antenna, long timestamp) =>
        $"m_plane_preaggregate_{node}_{antenna}_{timestamp}";
}