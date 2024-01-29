using Common;
using Domain;
using DomainService;
using Infrastructure.RepositoryCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Test;


[TestClass]
public class CompilerDomainServiceTests
{
    private Mock<IPlaneCacheRepository> _planeCacheRepositoryMock;
    private Mock<IPlaneMetadataRepository> _planeMetadataRepositoryMock;
    private Mock<IPlaneFramePublisher> _planeFramePublisherMock;
    private Mock<ILogger<CompilerDomainService>> _loggerMock;
    private Mock<IOptions<TimingsOptions>> _optionsMock;
    private CompilerDomainService _domainService;
    
    [TestInitialize]
    public void InitializeTest()
    {
        _loggerMock = new();         
        _planeCacheRepositoryMock = new();
        _planeMetadataRepositoryMock = new();
        _planeFramePublisherMock = new();
        _optionsMock = new();
        
        _optionsMock.Setup(_ => _.Value).Returns(new TimingsOptions
        {
            PlaneDocLifetimesSecs = 60,
            CompilationDurationPredictionSecs = 10,
            CompilationOffsetSecs = 1
        });
        
        _domainService = new(
            _planeCacheRepositoryMock.Object,
            _planeMetadataRepositoryMock.Object,
            _planeFramePublisherMock.Object,
            _optionsMock.Object,
            _loggerMock.Object
            );
    }
    
    [TestMethod]
    public async Task CompilerDomainService_StandardUse_Pass()
    {
        var planeFrame1 = new PlaneFrame()
        {
            Now = 1,
            Antenna = "a",
            Source = "b",
            Planes = new[] { new TimeAnotatedPlane()
            {
                Squawk = "planeframe1squawk",
                SquawkUpdated = 1,
                Flight = "planeframe1flight",
                FlightUpdated = 0,
                Latitude = 1,
                Longitude = 1,
                PositionUpdated = 1,
                Altitude = 1,
                AltitudeUpdated = 0,
                VerticleRate = 1,
                VerticleRateUpdated = 1,
                Track = 1,
                TrackUpdated = 0,
                Speed = 1,
                SpeedUpdated = 1,
                Category = "cat1",
                CategoryUpdated = 0,
                HexValue = "plane"
                
            }}

        };
        var planeFrame2 = new PlaneFrame()
        {
            Now = 1,
            Antenna = "a",
            Source = "b",
            Planes = new[] { new TimeAnotatedPlane()
            {
                Squawk = "planeframe2squawk",
                SquawkUpdated = 0,
                Flight = "planeframe2flight",
                FlightUpdated = 2,
                Latitude = 2,
                Longitude = 2,
                PositionUpdated = 0,
                Altitude = 2,
                AltitudeUpdated = 2,
                VerticleRate = 2,
                VerticleRateUpdated = 0,
                Track = 2,
                TrackUpdated = 2,
                Speed = 2,
                SpeedUpdated = 0,
                Category = "cat2",
                CategoryUpdated = 2,
                HexValue = "plane"
                
            }}

        };
        var planeframeExpected = new PlaneFrame()
        {
            Now = 1,
            Antenna = "congregator",
            Source = "congregation",
            Planes = new[]
            {
                new TimeAnotatedPlane()
                {
                    Squawk = "planeframe1squawk",
                    SquawkUpdated = 1,
                    Flight = "planeframe2flight",
                    FlightUpdated = 2,
                    Latitude = 1,
                    Longitude = 1,
                    PositionUpdated = 1,
                    Altitude = 2,
                    AltitudeUpdated = 2,
                    VerticleRate = 1,
                    VerticleRateUpdated = 1,
                    Track = 2,
                    TrackUpdated = 2,
                    Speed = 1,
                    SpeedUpdated = 1,
                    Category = "cat2",
                    CategoryUpdated = 2,
                    HexValue = "plane",
                    Messages = "0",
                    Rssi = 0
                }

            }
        };
        _planeCacheRepositoryMock
            .Setup(_ => _.CollectPlaneStatesAsync(It.IsAny<long>()))
            .Returns(GetPlanes(new[] {planeFrame1, planeFrame2}));
        

        await _domainService.CompilePlanesAsync(1);
        
        _planeCacheRepositoryMock
            .Verify(_ => _.InsertCompiledPlaneFrameAsync(
                It.Is<PlaneFrame>(_ => ComparePlaneFrame(planeframeExpected,_))),
                Times.Once);
        
    }
    private bool ComparePlaneFrame(PlaneFrame expected, PlaneFrame actual)
    {
        Assert.AreEqual(expected.Antenna, actual.Antenna);
        Assert.AreEqual(expected.Source, actual.Source);
        var expectedPlanes = expected.Planes.ToDictionary(_ => _.HexValue);
        var actualPlanes = actual.Planes.ToDictionary(_ => _.HexValue);
        
        Assert.AreEqual(expectedPlanes.Count, actualPlanes.Count);
        
        foreach(var planeHex in expectedPlanes.Keys)
        {
            ComparePlane(expectedPlanes[planeHex],actualPlanes[planeHex]);
        }
        
        return true;
        
    }
    
    private bool ComparePlane(Plane expected, Plane actual)
    {
        Assert.AreEqual(expected.Squawk, actual.Squawk, "Squawk");
        Assert.AreEqual(expected.HexValue, actual.HexValue, "HexValue");
        Assert.AreEqual(expected.Squawk, actual.Squawk, "Squawk");
        Assert.AreEqual(expected.Flight, actual.Flight, "Flight");
        Assert.AreEqual(expected.Latitude, actual.Latitude, "Latitude");
        Assert.AreEqual(expected.Longitude, actual.Longitude, "Longitude");
        Assert.AreEqual(expected.Nucp, actual.Nucp, "Nucp");
        Assert.AreEqual(expected.Altitude, actual.Altitude, "Altitude");
        Assert.AreEqual(expected.VerticleRate, actual.VerticleRate, "VerticleRate");
        Assert.AreEqual(expected.Track, actual.Track, "Track");
        Assert.AreEqual(expected.Speed, actual.Speed, "Speed");
        Assert.AreEqual(expected.Category, actual.Category, "Category");
        Assert.AreEqual(expected.Messages, actual.Messages, "Messages");
        Assert.AreEqual(expected.Rssi, actual.Rssi, "Rssi");
        
        return true;
    }
    private async IAsyncEnumerable<PlaneFrame> GetPlanes(IEnumerable<PlaneFrame> frames)
    {
        foreach(var planeFrame in frames)
        {
            await Task.Delay(1);
            yield return planeFrame;
        }
    }
}