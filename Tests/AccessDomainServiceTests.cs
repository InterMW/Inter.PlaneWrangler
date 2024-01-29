using DomainService;
using Infrastructure.RepositoryCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Tests;

[TestClass]
public class AccessDomainServiceTests
{
    private Mock<IPlaneCacheRepository> _mockPlaneCacheRepository; 
    private AccessDomainService _service;
    
    [TestInitialize]
    public void InitializeTest()
    {
        _mockPlaneCacheRepository = new();
        _service = new(_mockPlaneCacheRepository.Object);
    }
    
    [TestMethod]
    public async Task AccessDomainService_StandardUse_Pass()
    {
        await _service.RetrieveRecentPlaneFrame(0);
        _mockPlaneCacheRepository.Verify(_ => _.GetCompiledPlaneFrameAsync(0),Times.Once);
    }
}