using Domain;
using Infrastructure.Rabbit.Mappers;
using Infrastructure.Rabbit.Messages;
using Infrastructure.RepositoryCore;
using MelbergFramework.Infrastructure.Rabbit.Publishers;

namespace Infrastructure.Rabbit.Publishers;

public class PlaneFramePublisher : IPlaneFramePublisher
{
    private readonly IStandardPublisher<CompletedPlaneFrameMessage> _publisher;

    public PlaneFramePublisher(IStandardPublisher<CompletedPlaneFrameMessage> publisher)
    {
        _publisher = publisher;
    }

    public void PublishPlaneFrame(PlaneFrame frame) => 
        _publisher.Send(frame.ToMessage());
}