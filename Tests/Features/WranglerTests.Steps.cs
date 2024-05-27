using Application;
using Application.Models;
using Application.Responses;
using Common;
using DomainService;
using MelbergFramework.ComponentTesting.Rabbit;
using MelbergFramework.Core.ComponentTesting;
using MelbergFramework.Core.Time;
using MelbergFramework.Infrastructure.Rabbit.Extensions;
using MelbergFramework.Infrastructure.Rabbit.Messages;
using MelbergFramework.Infrastructure.Rabbit.Translator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Features;

public partial class WranglerTests : BaseTestFrame
{
    private Dictionary<int, PlaneFrameMessage> Scenarios =
        new Dictionary<int, PlaneFrameMessage>();

    private IActionResult _response;
    private AirplaneData _plane1 = new AirplaneData 
                {
                    id = 1,
                    hex = "abcdee",
                    lat = 1,
                    lon = 2,
                    nucp = "1",
                    rssi = 3,
                    speed = 4,
                    track = 1,
                    flight = "a",
                    squawk = "b",
                    altitude = 2,
                    category = "cat1",
                    vert_rate = 1,
                    vert_update = 1,
                    speed_update = 1,
                    messages="",
                    track_update = 1,
                    flight_update = 1,
                    squawk_update =  1,
                    altitude_update = 1,
                    category_update = 1,
                    position_update = 2
                };

    private AirplaneData _plane2 = 
                new AirplaneData 
                {
                    id = 1,
                    hex = "abcdef",
                    lat = 1,
                    lon = 2,
                    position_update = 2,
                    nucp = "1",
                    rssi = 8,
                    speed = 4,
                    speed_update = 1,
                    track = 10,
                    track_update = 2,
                    flight = "a",
                    flight_update = 1,
                    squawk = "b",
                    squawk_update =  11,
                    altitude = 2,
                    altitude_update = 1,
                    vert_rate = 2,
                    vert_update = 2,
                    messages="",
                    category = "cat1",
                    category_update = 1
                };

    private AirplaneData _plane3 = 
                new AirplaneData 
                {
                    id = 1,
                    hex = "abcdef",
                    lat = 2,
                    lon = 3,
                    position_update = 3,
                    nucp = "1",
                    rssi = 3,
                    speed = 41,
                    speed_update = 10,
                    track = 1,
                    track_update = 1,
                    flight = "b",
                    flight_update = 10,
                    squawk = "d",
                    squawk_update =  1,
                    altitude = 5,
                    altitude_update = 3,
                    vert_rate = 1,
                    vert_update = 1,
                    messages="",
                    category = "cat2",
                    category_update = 2
                };

    public async Task Setup_scenarios()
    {
        Scenarios.Add(1, new PlaneFrameMessage()
        {
            Now = 4,
            Planes = new []
            {
                _plane1,
                _plane2
            },
            Source = "1",
            Antenna = "antenna",
        });
        Scenarios.Add(2, new PlaneFrameMessage()
        {
            Now = 4,
            Planes = new []
            {
                _plane3
            },
            Source = "2",
            Antenna = "antenna",
        });
    
    }
    public async Task Planes_comes_in(int scenario)
    {
        var mockTranslator = (MockTranslator<PlaneFrameMessage>)GetClass<IJsonToObjectTranslator<PlaneFrameMessage>>();
        mockTranslator.Messages.Add( Scenarios[scenario]);
        await GetIngressService().ConsumeMessageAsync(new Message(),CancellationToken.None) ;
    }

    public async Task Compile_for_time(long time)
    {
        var message = new Message();
        message.Body = new byte[0];

        message.SetTimestamp(DateTime.UnixEpoch.AddSeconds(time + 1));
        await GetTickService().ConsumeMessageAsync(message, CancellationToken.None);
    }

    public async Task Get_planes()
    {
        var clock = (MockClock)GetClass<IClock>();
        clock.NewCurrentTime =DateTime.UnixEpoch.AddSeconds(7); 
        var controller = new WranglerController(
                GetClass<IAccessDomainService>(),
                GetClass<IOptions<TimingsOptions>>(),
                GetClass<IClock>());


       _response = await controller.GetFrameAsync(4);
    }

    public async Task The_right_planes_are_there()
    {
        var value = _response as OkObjectResult;

        Assert.AreEqual(200,value.StatusCode);
        
        Assert.IsInstanceOfType<PlaneFrameResponse>(value.Value);
        
        var frame = value.Value as PlaneFrameResponse;

        Assert.AreEqual(2,frame.Planes.Count());
        
        var planes = frame.Planes.ToDictionary(_ => _.HexValue);
        
        var planeabcdf = planes["abcdef"];
        Assert.AreEqual(2,planeabcdf.Latitude);
        Assert.AreEqual(3,planeabcdf.Longitude);
        Assert.AreEqual(41,planeabcdf.Speed);
        Assert.AreEqual(8,planeabcdf.Rssi);
        Assert.AreEqual(10,planeabcdf.Track);
        Assert.AreEqual("b",planeabcdf.Flight);
        Assert.AreEqual("b",planeabcdf.Squawk);
        Assert.AreEqual(5,planeabcdf.Altitude);
        Assert.AreEqual(2,planeabcdf.VerticleRate);
        Assert.AreEqual("cat2",planeabcdf.Category);
    }
}
