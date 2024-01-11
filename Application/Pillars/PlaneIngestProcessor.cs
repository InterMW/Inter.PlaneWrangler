using DomainService;
using MelbergFramework.Infrastructure.Rabbit.Consumers;
using MelbergFramework.Infrastructure.Rabbit.Messages;
using MelbergFramework.Infrastructure.Rabbit.Translator;
using Application.Mappers;
using Application.Models;

namespace Application.Pillars;
public class PlaneIngestProcessor : IStandardConsumer
{
    private readonly IPlaneIngestDomainService _domainService;
    private readonly IJsonToObjectTranslator<PlaneFrameMessage> _translator;
    private readonly ILogger _logger;

    public PlaneIngestProcessor(
        IPlaneIngestDomainService domainService,
        IJsonToObjectTranslator<PlaneFrameMessage> translator,
        ILogger logger
        )
    {
        _domainService = domainService;
        _translator = translator;
        _logger = logger;
    }

    public async Task ConsumeMessageAsync(Message message, CancellationToken ct) 
    {
        await _domainService.IngestPlaneFrameAsync(_translator.Translate(message).ToDomain());

        _logger.LogInformation("Handled ingest");
    }
}