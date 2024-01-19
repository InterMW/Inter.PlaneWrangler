using System.Diagnostics;
using DomainService;
using MelbergFramework.Infrastructure.Rabbit.Consumers;
using MelbergFramework.Infrastructure.Rabbit.Extensions;
using MelbergFramework.Infrastructure.Rabbit.Messages;

namespace Application.Pillars;

public class TickCommandProcessor : IStandardConsumer
{
    private readonly ICompilerDomainService _domainService;
    private readonly ILogger<TickCommandProcessor> _logger;

    public TickCommandProcessor(ICompilerDomainService domainService, ILogger<TickCommandProcessor> logger)
    {
        _domainService = domainService;
        _logger = logger;
    }

    public async Task ConsumeMessageAsync(Message message, CancellationToken ct)
    {
        var sto = new Stopwatch();
        sto.Start();
        await _domainService.CompilePlanesAsync(ExtractTimestamp(message.GetTimestamp()));
        sto.Stop();
        _logger.LogWarning($"Handled tick in {sto.ElapsedMilliseconds}");
    } 
    
    private long ExtractTimestamp(DateTime time) => 
        (long) Math.Floor(
            time.Subtract(DateTime.UnixEpoch).TotalSeconds
            );

}