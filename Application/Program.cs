using MelbergFramework.Application;

namespace Application;

public class Program
{
    public static async Task Main(string[] args)
    {
        ThreadPool.SetMinThreads(8, 8); //required

        await MelbergHost
            .CreateHost<AppRegistrator>()
            .DevelopmentPasswordReplacement("Rabbit:ClientDeclarations:Connections:0:Password", "rabbit_pass")
            .AddServices(_ =>
                    {
                        _.AddControllers();
                        _.AddSwaggerGen();
                    })
            .ConfigureApp(_ =>
                    {
                        _.UseSwagger();
                        _.UseSwaggerUI();
                        _.UseRouting();
                        _.MapControllers();
                    })
            .Build()
            .RunAsync();

    }
}
