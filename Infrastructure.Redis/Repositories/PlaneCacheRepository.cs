using System.Text.Json;
using Common;
using Domain;
using Infrastructure.Redis.Contexts;
using Infrastructure.RepositoryCore;
using MelbergFramework.Infrastructure.Redis;
using Microsoft.Extensions.Options;

namespace Infrastructure.Redis.Repositories;

public class PlaneCacheRepository : RedisRepository<PlaneCacheContext>, IPlaneCacheRepository
{
    private readonly TimeSpan _frameLifespan;

    public PlaneCacheRepository(
        PlaneCacheContext context,
        IOptions<TimingsOptions> options) : base(context)
    {
        _frameLifespan = new TimeSpan(0, 0, options.Value.PlaneDocLifetimesSecs);
    }

    public async Task InsertNodePlaneFrameAsync(PlaneFrame frame)
    {
        var precompileKey = ToPrecompiledKey(frame);
        await DB.SetAddAsync(precompileKey,
                JsonSerializer.Serialize(frame.Planes));
        await DB.KeyExpireAsync(precompileKey, _frameLifespan);
    }


    public Task InsertCompiledPlaneFrameAsync(PlaneFrame frame) =>
        DB.StringSetAsync(
            ToCompiledKey(frame),
            JsonSerializer.Serialize(frame),
            _frameLifespan
        );

    public async Task<IEnumerable<TimeAnotatedPlane>> CollectPlaneStatesAsync(long timestamp)
    {
        var planeFrames = await DB.SetMembersAsync(ToPrecompiledKey(timestamp));

        return planeFrames
            .Select(_ => JsonSerializer.Deserialize<TimeAnotatedPlane[]>(_))
            .SelectMany(_ => _);
    }

    public async Task<PlaneFrame> GetCompiledPlaneFrameAsync(long timestamp)
    {
        var payload = await DB.StringGetAsync(ToCompiledKey(timestamp));

        if(!payload.HasValue)
        {
            return new PlaneFrame() {Now = timestamp};
        }
        return JsonSerializer.Deserialize<PlaneFrame>(payload);
    }

    private string ToPrecompiledKey(PlaneFrame frame) =>
        ToPrecompiledKey(frame.Now);
    private string ToPrecompiledKey(long time) =>
        $"wrangle_precompiled_{time}";

    private string ToCompiledKey(PlaneFrame frame) => ToCompiledKey(frame.Now);
    private string ToCompiledKey(long time) => $"wrangle_plane_combinded_{time}";
}
