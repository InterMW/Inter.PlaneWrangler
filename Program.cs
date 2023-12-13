using Inter.Consumers;
using Inter.Domain;
using Inter.Services;
using MelbergFramework.Infrastructure.Rabbit;
using MelbergFramework.Infrastructure.Rabbit.Translator;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();

RabbitModule.RegisterMicroConsumer<PlaneConsumer>(builder.Services,"PlaneIngress");
builder.Services.AddTransient<IPlaneIngestService,PlaneIngestService>();
builder.Services.AddTransient<IJsonToObjectTranslator<PlaneFrame>,JsonToObjectTranslator<PlaneFrame>>();

RabbitModule.RegisterMicroConsumer<TickConsumer>(builder.Services, "Tick");
builder.Services.AddTransient<ILogger>(s => s.GetRequiredService<ILogger<Program>>());

var app = builder.Build();

app.MapGet("/", () => 0);

await app.RunAsync();

