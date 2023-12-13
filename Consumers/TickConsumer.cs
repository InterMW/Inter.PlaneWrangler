using MelbergFramework.Infrastructure.Rabbit.Consumers;
using MelbergFramework.Infrastructure.Rabbit.Messages;

public class TickConsumer : IStandardConsumer
{
    public TickConsumer()
    {
    }

    public async Task ConsumeMessageAsync(Message message, CancellationToken ct)
    {

    }
}