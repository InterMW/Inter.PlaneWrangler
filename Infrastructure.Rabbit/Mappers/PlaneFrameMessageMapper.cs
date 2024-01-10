using Domain;
using Infrastructure.Rabbit.Messages;

namespace Infrastructure.Rabbit.Mappers;

public static class PlaneFrameMessageMapper 
{
    public static CompletedPlaneFrameMessage ToMessage(this PlaneFrame frame) => new()
    {
        Planes = frame.Planes.Select(_ => (Plane) _),
        Now = frame.Now
    };
}