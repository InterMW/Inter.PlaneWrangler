using Application.Filters;
using Application.Models;
using Application.Pillars;
using Common;
using DomainService;
using Infrastructure.InfluxDB.Contexts;
using Infrastructure.InfluxDB.Repositories;
using Infrastructure.Rabbit.Messages;
using Infrastructure.Rabbit.Publishers;
using Infrastructure.Redis.Contexts;
using Infrastructure.Redis.Repositories;
using Infrastructure.RepositoryCore;
using MelbergFramework.Application.Health;
using MelbergFramework.Core.Health;
using MelbergFramework.Infrastructure.InfluxDB;
using MelbergFramework.Infrastructure.Rabbit;
using MelbergFramework.Infrastructure.Redis;
using System.Diagnostics;
using System.Threading;

namespace Application;

public class Program
{
    public static void Main(string[] args) 
    {
        //Fix latency, its huge
        ThreadPool.SetMinThreads(40, 40);
        var builder = WebApplication.CreateBuilder();
        
        builder.Services.AddControllers().AddNewtonsoftJson();

        RegisterDependencies(builder.Services, builder.Configuration);
        var app = builder.Build();
        

        if(app.Environment.IsDevelopment())
        {
            app.Configuration["Rabbit:ClientDeclarations:Connections:0:Password"] = app.Configuration["rabbit_pass"];
        } 
        
            app.UseHttpsRedirection();

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
            app.UseCors(_ => _
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials()
                );
        app.UseSwagger();
        app.UseSwaggerUI();

        app.Run();
    }
    
    private static void RegisterDependencies(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<HealthCheckBackgroundService>();
        services.AddSingleton<IHealthCheckChecker,HealthCheckChecker>();

        RabbitModule.RegisterMicroConsumer<PlaneIngestProcessor,PlaneFrameMessage>(services);
        RabbitModule.RegisterMicroConsumer<TickCommandProcessor,TickMessage>(services);

        services.AddScoped<IActionResponseTimeStopwatch, ActionResponseTimeStopwatch>();
        services.AddMvc(options =>
        {
            options.Filters.Add(new ResponseTimeFilter());
        }) ;
        services.AddTransient<IPlaneIngestDomainService,PlaneIngestDomainService>();
        services.AddTransient<ICompilerDomainService, CompilerDomainService>();
        services.AddTransient<IAccessDomainService,AccessDomainService>();

        RedisModule.LoadRedisRepository<IPlaneCacheRepository,PlaneCacheRepository,PlaneCacheContext>(services);
        InfluxDBModule.LoadInfluxDBRepository<IPlaneMetadataRepository,PlaneFrameMetadataRepository,InfluxDBContext>(services);

        services.AddTransient<IPlaneFramePublisher,PlaneFramePublisher>();
        RabbitModule.RegisterPublisher<CompletedPlaneFrameMessage>(services);

        services.AddSingleton<ILogger>(_ => _.GetRequiredService<ILogger<int>>());
        
        services.AddOptions<TimingsOptions>()
            .BindConfiguration(TimingsOptions.Timing)
            .ValidateDataAnnotations();

        services.AddSingleton<TimingsOptions,TimingsOptions>();
        services.AddSwaggerGen();
    }

}