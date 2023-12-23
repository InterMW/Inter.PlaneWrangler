using Domain;

public interface IPlaneMetadataRepository
{
    Task LogPlaneMetadata(PlaneFrameMetadata metadata);
}