using Domain;

namespace Infrastructure.RepositoryCore;

public interface IPlaneFramePublisher
{
    void PublishPlaneFrame(PlaneFrame frame);
}