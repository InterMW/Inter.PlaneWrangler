using DomainService;
using MelbergFramework.Infrastructure.Rabbit.Consumers;
using MelbergFramework.Infrastructure.Rabbit.Extensions;
using MelbergFramework.Infrastructure.Rabbit.Messages;

namespace Application.Pillars;

public class TickCommandProcessor : IStandardConsumer
{
    private readonly ICompilerDomainService _domainService;
    private readonly ILogger<TickCommandProcessor> _logger;

    public TickCommandProcessor(ICompilerDomainService domainService,
            ILogger<TickCommandProcessor> logger)
    {
        _domainService = domainService;
        _logger = logger;
    }

    public async Task ConsumeMessageAsync(Message message, CancellationToken ct)
    {
        var time = ExtractTimestamp(message.GetTimestamp());
        await _domainService.CompilePlanesAsync(time);
        _logger.LogWarning("Handled tick for moment {past} ",time);
    } 
    
    private long ExtractTimestamp(DateTime time) => 
        (long) Math.Floor( time.Subtract(DateTime.UnixEpoch).TotalSeconds);
}
