using Domain;
using MelbergFramework.Infrastructure.Rabbit.Messages;

namespace Infrastructure.Rabbit.Messages;

public class CompletedPlaneFrameMessage : StandardMessage
{
    public IEnumerable<Plane> Planes = Array.Empty<Plane>();
    public long Now;
    public override string GetRoutingKey() => "planeframe.complete";
}