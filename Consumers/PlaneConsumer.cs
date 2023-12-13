using Inter.Consumers.Models;
using Inter.Consumers.Mappers;
using Inter.Services;
using MelbergFramework.Infrastructure.Rabbit.Consumers;
using MelbergFramework.Infrastructure.Rabbit.Messages;
using MelbergFramework.Infrastructure.Rabbit.Translator;

namespace Inter.Consumers;

public class PlaneConsumer : IStandardConsumer
{
    private readonly IJsonToObjectTranslator<AirplaneRecord> _translator;
    private readonly IPlaneIngestService _domainService; 
    public PlaneConsumer(
        IPlaneIngestService domainService,
        IJsonToObjectTranslator<AirplaneRecord> translator)
    {
        _domainService = domainService;
        _translator = translator;
    }
    public Task ConsumeMessageAsync(Message message, CancellationToken ct) =>
       _domainService.IngestPlaneFrameAsync(_translator.Translate(message).ToDomain());
        
}