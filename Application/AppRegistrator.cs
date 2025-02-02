using Application.Models;
using Application.Pillars;
using DomainService;
using MelbergFramework.Application;
using MelbergFramework.Infrastructure.Rabbit;
using MelbergFramework.Infrastructure.InfluxDB;
using Infrastructure.InfluxDB.Repositories; 
using Infrastructure.InfluxDB.Contexts;
using MelbergFramework.Infrastructure.Redis;
using Infrastructure.RepositoryCore;
using Infrastructure.Redis.Repositories;
using Infrastructure.Redis.Contexts;
using Infrastructure.Rabbit.Publishers;
using Infrastructure.Rabbit.Messages;
using Common;
using MelbergFramework.Core.Time;
using Device.GrpcClient;

namespace Application;

public class AppRegistrator : Registrator
{
    public override void RegisterServices(IServiceCollection services)
    {
        RabbitModule.RegisterMicroConsumer<PlaneIngestProcessor,
            PlaneFrameMessage>(services,true);
        RabbitModule.RegisterMicroConsumer<
            TickCommandProcessor,
            MelbergFramework.Infrastructure.Rabbit.Messages.TickMessage>(services, true);

        DeviceGrpcDependencyModule.RegisterClient(services);
        services.AddTransient<IPlaneIngestDomainService,PlaneIngestDomainService>();
        services.AddTransient<ICompilerDomainService, CompilerDomainService>();
        services.AddTransient<IAccessDomainService,AccessDomainService>();

        RedisDependencyModule.LoadRedisRepository<IPlaneCacheRepository,PlaneCacheRepository,PlaneCacheContext>(services);
        InfluxDBDependencyModule.LoadInfluxDBRepository<IPlaneMetadataRepository,PlaneFrameMetadataRepository,InfluxDBContext>(services);

        services.AddTransient<IPlaneFramePublisher,PlaneFramePublisher>();
        RabbitModule.RegisterPublisher<CompletedPlaneFrameMessage>(services);
        
        services.AddOptions<TimingsOptions>()
            .BindConfiguration(TimingsOptions.Timing)
            .ValidateDataAnnotations();

        services.AddSwaggerGen();

        services.AddSingleton<IClock, Clock>();
    }
}
