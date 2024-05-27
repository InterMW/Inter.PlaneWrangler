using Application;
using Application.Models;
using Application.Pillars;
using Common;
using Infrastructure.InfluxDB.Contexts;
using Infrastructure.Rabbit.Messages;
using Infrastructure.Rabbit.Publishers;
using Infrastructure.Redis.Contexts;
using LightBDD.MsTest3;
using MelbergFramework.Application;
using MelbergFramework.ComponentTesting.Rabbit;
using MelbergFramework.ComponentTesting.Redis;
using MelbergFramework.Core.ComponentTesting;
using MelbergFramework.Core.DependencyInjection;
using MelbergFramework.Core.Time;
using MelbergFramework.Infrastructure.Rabbit.Messages;
using MelbergFramework.Infrastructure.Rabbit.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Tests.Features;

public class BaseTestFrame : FeatureFixture
{
    public BaseTestFrame()
    {
        App = MelbergHost
                .CreateHost<AppRegistrator>()
                .AddServices(_ => 
                {
                    _.PrepareConsumer<TickCommandProcessor>(); 
                    _.OverrideTranslator<TickMessage>();
                    _.PrepareConsumer<PlaneIngestProcessor>(); 
                    _.OverrideTranslator<PlaneFrameMessage>();
                    _.OverrideWithSingleton<IClock,MockClock>();
                    _.Configure<TimingsOptions>( _ => 
                        {
                            _.CompilationOffsetSecs = 1;
                            _.PlaneDocLifetimesSecs = 45;
                            _.CompilationDurationPredictionSecs = 1;
                        });
                    _.OverrideRedisContext<PlaneCacheContext>();
                    _.OverridePublisher<CompletedPlaneFrameMessage>();
                    MockInfluxDBDependencyModule.MockInfluxDBContext<InfluxDBContext>(_);
                })
                .AddControllers()
                .Build();

    }

    public WebApplication App;

    public T GetClass<T>() => (T)App
        .Services
        .GetRequiredService(typeof(T));

    public RabbitMicroService<PlaneIngestProcessor> GetIngressService() =>
        (RabbitMicroService<PlaneIngestProcessor>)App
            .Services
            .GetServices<IHostedService>()
            .Where(_ => _.GetType() == typeof(RabbitMicroService<PlaneIngestProcessor>))
            .First();

    public RabbitMicroService<TickCommandProcessor> GetTickService() =>
        (RabbitMicroService<TickCommandProcessor>)App
            .Services
            .GetServices<IHostedService>()
            .Where(_ => _.GetType() == typeof(RabbitMicroService<TickCommandProcessor>))
            .First();
}
