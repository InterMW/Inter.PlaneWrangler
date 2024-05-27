using System.Text.Json.Serialization;
using MelbergFramework.Infrastructure.Rabbit.Messages;

namespace Application.Models;

public class PlaneFrameMessage : StandardMessage
{
    [JsonPropertyName("now")]
    public double Now {get; set;}
    [JsonPropertyName("aircraft")]
    public AirplaneData[] Planes {get; set;} = Array.Empty<AirplaneData>();
    [JsonPropertyName("source")]
    public string Source {get; set;} = string.Empty;
    [JsonPropertyName("antenna")]
    public string Antenna {get; set;} = string.Empty;
    public override string GetRoutingKey() => "plane.final";
}
