using Domain;
using DomainService;
using Infrastructure.RepositoryCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Tests;

[TestClass]
public class PlaneIngressDomainServiceTests
{
    private Mock<IPlaneCacheRepository> _mockPlaneCacheRepository;
    private Mock<IPlaneMetadataRepository> _mockPlaneMetadataRepository;
    private PlaneIngestDomainService _service;
    
    [TestInitialize]
    public void Initialize()
    {
       _mockPlaneCacheRepository = new(); 
       _mockPlaneMetadataRepository = new();
       
       _service = new(_mockPlaneCacheRepository.Object,_mockPlaneMetadataRepository.Object);
    }

    [TestMethod]
    public async Task PlaneIngestDomainService_StandardInput_Pass()
    {
        var plane = new TimeAnotatedPlane()
        {
            Altitude = 1,
            HexValue = "a"
        };
        var frame = new PlaneFrame()
        {
            Now = 1,
            Planes = new[]
            {
                plane
            }
        };
        
        await _service.IngestPlaneFrameAsync(frame);
        
        _mockPlaneCacheRepository.Verify(_ => _.InsertNodePlaneFrameAsync(It.IsAny<PlaneFrame>()),Times.Once);
        _mockPlaneMetadataRepository.Verify(_ => _.LogPlaneMetadata(It.IsAny<PlaneFrameMetadata>()),Times.Once);
        
    }
}