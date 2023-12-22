using MelbergFramework.Infrastructure.Rabbit.Messages;
using Newtonsoft.Json;

namespace Application.Models;

public class PlaneFrameMessage : StandardMessage
{
    [JsonProperty("now")]
    public double Now {get; set;}
    [JsonProperty("aircraft")]
    public AirplaneData[] Planes {get; set;} = Array.Empty<AirplaneData>();
    [JsonProperty("source")]
    public string Source {get; set;} = string.Empty;
    [JsonProperty("antenna")]
    public string Antenna {get; set;} = string.Empty;
    public override string GetRoutingKey() => "plane.final";
}