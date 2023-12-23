using MelbergFramework.Infrastructure.InfluxDB;

namespace Infrastructure.InfluxDB.Contexts;

public class InfluxDBContext : DefaultContext
{
    public InfluxDBContext(IStandardInfluxDBClientFactory factory) : base(factory) { }
}