using MelbergFramework.Infrastructure.Rabbit.Messages;

namespace Application.Models;

public class TickMessage : StandardMessage
{
    public override string GetRoutingKey() => "";
}