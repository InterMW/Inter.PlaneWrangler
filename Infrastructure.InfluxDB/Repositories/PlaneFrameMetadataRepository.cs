using Domain;
using Infrastructure.InfluxDB.Contexts;
using Infrastructure.InfluxDB.Mappers;
using MelbergFramework.Infrastructure.InfluxDB;

namespace Infrastructure.InfluxDB.Repositories;

public class PlaneFrameMetadataRepository : BaseInfluxDBRepository<InfluxDBContext>, IPlaneMetadataRepository
{
    public PlaneFrameMetadataRepository(InfluxDBContext context) : base(context) { }

    public Task LogPlaneMetadata(PlaneFrameMetadata metadata) => 
        Context.WritePointAsync(
            metadata.ToDataModel(),
            "plane_data",
            "Inter");
}