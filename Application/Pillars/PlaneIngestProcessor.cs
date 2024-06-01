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
    private readonly ILogger<PlaneIngestProcessor> _logger;

    public PlaneIngestProcessor(
        IPlaneIngestDomainService domainService,
        IJsonToObjectTranslator<PlaneFrameMessage> translator,
        ILogger<PlaneIngestProcessor> logger
        )
    {
        _domainService = domainService;
        _translator = translator;
        _logger = logger;
    }

    public async Task ConsumeMessageAsync(Message message, CancellationToken ct) 
    {
        var planeFrame = _translator.Translate(message)!;
        await _domainService.IngestPlaneFrameAsync(planeFrame.ToDomain());

        _logger.LogInformation("Consumed plane frame from {node}", planeFrame.Source);
    }
}
