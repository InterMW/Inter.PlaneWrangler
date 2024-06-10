using System.Text.Json.Serialization;

namespace Application.Responses;
public class PlaneResponse 
{
    [JsonPropertyName("hexValue")] 
    public string HexValue {get; set;} = string.Empty;
    [JsonPropertyName("squawk")]
    public string Squawk {get; set;} = string.Empty;
    [JsonPropertyName("flight")]
    public string Flight {get; set;} = string.Empty;
    [JsonPropertyName("lat")]
    public float? Latitude {get; set;}
    [JsonPropertyName("lon")]
    public float? Longitude {get; set;}
    [JsonPropertyName("nucp")]
    public string Nucp {get; set;} = string.Empty;
    [JsonPropertyName("altitude")]
    public int? Altitude {get; set;}
    [JsonPropertyName("vert_rate")]
    public int? VerticleRate {get; set;}
    [JsonPropertyName("track")]
    public int? Track {get; set;}
    [JsonPropertyName("speed")]
    public int? Speed {get; set;}
    [JsonPropertyName("category")]
    public string Category {get; set;} = string.Empty;
    [JsonPropertyName("messages")]
    public string Messages {get; set;} = string.Empty;
    [JsonPropertyName("rssi")] 
    public float? Rssi {get;set;}
}
