using Application.Models;
using Application.Pillars;
using Common;
using DomainService;
using Infrastructure.InfluxDB.Contexts;
using Infrastructure.InfluxDB.Repositories;
using Infrastructure.Redis.Contexts;
using Infrastructure.Redis.Repositories;
using Infrastructure.RepositoryCore;
using MelbergFramework.Infrastructure.InfluxDB;
using MelbergFramework.Infrastructure.Rabbit;
using MelbergFramework.Infrastructure.Redis;

namespace Application;

public class Program
{
    public static void Main(string[] args) 
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddControllers();

        
        
        RegisterDependencies(builder.Services, builder.Configuration);
        var app = builder.Build();

        app.UseRouting();
        app.MapGet("/", () => "Hello World!");
        app.UseEndpoints( endpoints =>
        {
            endpoints.MapControllers();
        });
        app.UseSwagger();
        app.UseSwaggerUI();

        app.Run();
    }
    
    private static void RegisterDependencies(IServiceCollection services, IConfiguration configuration)
    {
        RabbitModule.RegisterMicroConsumer<PlaneIngestProcessor,PlaneFrameMessage>(services);
        RabbitModule.RegisterMicroConsumer<TickCommandProcessor,TickMessage>(services);
        
        services.AddTransient<IPlaneIngestDomainService,PlaneIngestDomainService>();
        services.AddTransient<ICompilerDomainService, CompilerDomainService>();
        services.AddTransient<IAccessDomainService,AccessDomainService>();

        RedisModule.LoadRedisRepository<IPlaneCacheRepository,PlaneCacheRepository,PlaneCacheContext>(services);
        InfluxDBModule.LoadInfluxDBRepository<IPlaneMetadataRepository,PlaneFrameMetadataRepository,InfluxDBContext>(services);

        services.AddSingleton<ILogger>(_ => _.GetRequiredService<ILogger<int>>());
        
        //
        // This doesn't work and it makes me sad
        //
        //services.Configure<TimingsOptions>(configuration.GetSection(TimingsOptions.Timing));
        //
        
        services.AddSingleton<TimingsOptions,TimingsOptions>();
        services.AddSwaggerGen();

                
    }
}
