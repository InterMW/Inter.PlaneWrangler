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
using MelbergFramework.Application;
using MelbergFramework.Infrastructure.InfluxDB;
using MelbergFramework.Infrastructure.Rabbit;
using MelbergFramework.Infrastructure.Redis;

namespace Application;

public class Program
{
    public static void Main(string[] args) 
    {
        ThreadPool.SetMinThreads(8, 8); //required
        var builder = WebApplication.CreateBuilder();
        
        builder.Services.AddControllers().AddNewtonsoftJson();

        RegisterDependencies(builder.Services,builder.Environment.IsDevelopment() );
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
    
    private static void RegisterDependencies(IServiceCollection services, bool isDevelopment)
    {
        RabbitModule.RegisterMicroConsumer<PlaneIngestProcessor,PlaneFrameMessage>(services,!isDevelopment);
        RabbitModule.RegisterMicroConsumer<
            TickCommandProcessor,
            MelbergFramework.Infrastructure.Rabbit.Messages.TickMessage>(services, !isDevelopment);

        services.RegisterRequired();
        services.AddScoped<IActionResponseTimeStopwatch, ActionResponseTimeStopwatch>();
        services.AddMvc(options =>
        {
            options.Filters.Add(new ResponseTimeFilter());
        });

        services.AddTransient<IPlaneIngestDomainService,PlaneIngestDomainService>();
        services.AddTransient<ICompilerDomainService, CompilerDomainService>();
        services.AddTransient<IAccessDomainService,AccessDomainService>();

        RedisModule.LoadRedisRepository<IPlaneCacheRepository,PlaneCacheRepository,PlaneCacheContext>(services);
        InfluxDBModule.LoadInfluxDBRepository<IPlaneMetadataRepository,PlaneFrameMetadataRepository,InfluxDBContext>(services);

        services.AddTransient<IPlaneFramePublisher,PlaneFramePublisher>();
        RabbitModule.RegisterPublisher<CompletedPlaneFrameMessage>(services);
        
        services.AddOptions<TimingsOptions>()
            .BindConfiguration(TimingsOptions.Timing)
            .ValidateDataAnnotations();

        services.AddSingleton<TimingsOptions,TimingsOptions>();
        services.AddSwaggerGen();
    }
}
