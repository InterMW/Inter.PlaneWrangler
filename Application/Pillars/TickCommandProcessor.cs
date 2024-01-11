using DomainService;
using MelbergFramework.Infrastructure.Rabbit.Consumers;
using MelbergFramework.Infrastructure.Rabbit.Messages;

namespace Application.Pillars;

public class TickCommandProcessor : IStandardConsumer
{
    private readonly ICompilerDomainService _domainService;
    private readonly ILogger _logger;

    public TickCommandProcessor(ICompilerDomainService domainService, ILogger logger)
    {
        _domainService = domainService;
        _logger = logger;
    }

    public async Task ConsumeMessageAsync(Message message, CancellationToken ct)
    {
        await _domainService.CompilePlanesAsync(ExtractTimestamp(message.Timestamp));

        _logger.LogInformation("Handled tick");
    } 
    
    private long ExtractTimestamp(DateTime time) => 
        (long) Math.Floor(
            time.Subtract(DateTime.UnixEpoch).TotalSeconds
            );

}