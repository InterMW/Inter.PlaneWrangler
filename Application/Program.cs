using MelbergFramework.Application;

namespace Application;

public class Program
{
    public static async Task Main(string[] args) 
    {
        ThreadPool.SetMinThreads(8, 8); //required

        var test = MelbergHost
            .CreateHost<AppRegistrator>()
            .DevelopmentPasswordReplacement("Rabbit:ClientDeclarations:Connections:0:Password", "rabbit_pass")
            .AddControllers()
            .Build().UseCors(_ => _.AllowAnyOrigin());
        var eout = test.Build();
            await eout.RunAsync();
       
    }
}
