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

    public PlaneIngestProcessor(
        IPlaneIngestDomainService domainService,
        IJsonToObjectTranslator<PlaneFrameMessage> translator
        )
    {
        _domainService = domainService;
        _translator = translator;
    }

    public Task ConsumeMessageAsync(Message message, CancellationToken ct) =>
        _domainService.IngestPlaneFrameAsync(_translator.Translate(message).ToDomain());
}