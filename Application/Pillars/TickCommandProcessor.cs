using DomainService;
using MelbergFramework.Infrastructure.Rabbit.Consumers;
using MelbergFramework.Infrastructure.Rabbit.Messages;

namespace Application.Pillars;

public class TickCommandProcessor : IStandardConsumer
{
    private readonly ICompilerDomainService _domainService;

    public TickCommandProcessor(ICompilerDomainService domainService)
    {
        _domainService = domainService;
    }

    public Task ConsumeMessageAsync(Message message, CancellationToken ct) =>
        _domainService.CompilePlanesAsync(ExtractTimestamp(message.Timestamp));
    
    private long ExtractTimestamp(DateTime time) => 
        (long) Math.Floor(
            time.Subtract(DateTime.UnixEpoch).TotalSeconds
            );

}