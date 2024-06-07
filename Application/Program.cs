using MelbergFramework.Application;

namespace Application;

public class Program
{
    public static async Task Main(string[] args)
    {
        ThreadPool.SetMinThreads(8, 8); //required
        var cors = "CORS";

        await MelbergHost
            .CreateHost<AppRegistrator>()
            .DevelopmentPasswordReplacement("Rabbit:ClientDeclarations:Connections:0:Password", "rabbit_pass")
            .AddServices(_ =>
                    {
                        _.AddControllers();
                        _.AddSwaggerGen();
                        _.AddCors(options =>
                        {
                            options.AddPolicy(name: cors,
                                              policy =>
                                              {
                                                  policy.WithOrigins("http://plane.centurionx.net", "https://plane.centurionx.net");
                                              });
                        });
                    })
            .ConfigureApp(_ =>
                    {
                        _.UseSwagger();
                        _.UseSwaggerUI();
                        _.UseRouting();
                        _.UseCors(cors);
                        _.MapControllers();
                    })
            .Build()
            .RunAsync();

    }
}
