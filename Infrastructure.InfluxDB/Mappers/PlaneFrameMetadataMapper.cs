using Domain;
using MelbergFramework.Infrastructure.InfluxDB;

namespace Infrastructure.InfluxDB.Mappers;

public static class PlaneFrameMetadataMapper 
{
    public static InfluxDBDataModel ToDataModel(this PlaneFrameMetadata metadata)
    {
        if(metadata == null)
        {
            return null;
        }

        var result = new InfluxDBDataModel("plane_metadata");

        result.Tags["antenna"] = metadata.Antenna;
        result.Tags["hostname"] = metadata.Hostname;
        result.Fields["total"] = metadata.Total;
        result.Fields["detailed"] = metadata.Detailed;
        result.Fields["average_distance"] = metadata.AverageDistance;
        result.Fields["max_distance"] = metadata.MaxDistance;
        result.Timestamp = metadata.Timestamp;
        return result;
    }
}
