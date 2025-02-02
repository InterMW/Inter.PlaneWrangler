using MelbergFramework.Infrastructure.InfluxDB;
using Microsoft.Extensions.Options;

namespace Infrastructure.InfluxDB.Contexts;

public class InfluxDBContext : DefaultContext
{
    public InfluxDBContext(
        IStandardClientFactory factory,
        IOptions<InfluxDBOptions<InfluxDBContext>> options) 
        : base(factory, options.Value){}
}
