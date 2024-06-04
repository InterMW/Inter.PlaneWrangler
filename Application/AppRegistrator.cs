using Application.Models;
using Application.Pillars;
using DomainService;
using MelbergFramework.Application;
using MelbergFramework.Infrastructure.Redis;
using Infrastructure.RepositoryCore;
using Infrastructure.Redis.Repositories;
using Infrastructure.Redis.Contexts;
using Common;
using MelbergFramework.Core.Time;

namespace Application;

public class AppRegistrator : Registrator
{
    public override void RegisterServices(IServiceCollection services)
    {
        // RabbitModule.RegisterMicroConsumer<PlaneIngestProcessor,
        //     PlaneFrameMessage>(services,true);
        // RabbitModule.RegisterMicroConsumer<
        //     TickCommandProcessor,
        //     MelbergFramework.Infrastructure.Rabbit.Messages.TickMessage>(services, true);

        services.AddTransient<IPlaneIngestDomainService,PlaneIngestDomainService>();
        services.AddTransient<ICompilerDomainService, CompilerDomainService>();
        services.AddTransient<IAccessDomainService,AccessDomainService>();

        RedisDependencyModule.LoadRedisRepository<IPlaneCacheRepository,PlaneCacheRepository,PlaneCacheContext>(services);

        
        services.AddOptions<TimingsOptions>()
            .BindConfiguration(TimingsOptions.Timing)
            .ValidateDataAnnotations();

        services.AddSwaggerGen();

        services.AddSingleton<IClock, Clock>();
    }
}
