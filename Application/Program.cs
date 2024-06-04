var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/wringler/test", () => "Hello World!");

app.Run();
