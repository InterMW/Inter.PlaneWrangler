using Inter.Domain;
using Inter.Infrastructure.Service;

namespace Inter.Services;

public interface IPlaneIngestService
{
    Task IngestPlaneFrameAsync(PlaneFrame frame);
}

public class PlaneIngestService : IPlaneIngestService
{
    private readonly IInfrastructureService _infrastructureService;
    public PlaneIngestService(IInfrastructureService infrastructureService)
    {
       _infrastructureService = infrastructureService; 
    }
    public Task IngestPlaneFrameAsync(PlaneFrame frame)
    {
        var ingestTask = _infrastructureService.InsertRawPlaneFrameAsync(frame);
        var metadataTask = HandleMetadata(frame);
        
        return Task.WhenAll(ingestTask, metadataTask);
    }
    private Task HandleMetadata(PlaneFrame frame) =>
        _infrastructureService.UploadPlaneFrameMetadataAsync( new PlaneFrameMetadata
        {
            Total = frame.Planes.Count(),
            Detailed = frame.Planes.Where(_ => _.Latitude.HasValue && _.Longitude.HasValue).Count(),

            Antenna = frame.Antenna,
            Hostname = frame.Source,
            Timestamp = DateTime.UnixEpoch.AddSeconds(frame.Now)
        });
}