using Newtonsoft.Json;

namespace Application.Responses;
public class PlaneResponse 
{
    [JsonProperty("hexValue")] 
    public string HexValue {get; set;} = string.Empty;
    [JsonProperty("squawk")]
    public string Squawk {get; set;} = string.Empty;
    [JsonProperty("flight")]
    public string Flight {get; set;} = string.Empty;
    [JsonProperty("lat")]
    public float? Latitude {get; set;}
    [JsonProperty("lon")]
    public float? Longitude {get; set;}
    [JsonProperty("nucp")]
    public string Nucp {get; set;} = string.Empty;
    [JsonProperty("altitude")]
    public int? Altitude {get; set;}
    [JsonProperty("vert_rate")]
    public int? VerticleRate {get; set;}
    [JsonProperty("track")]
    public int? Track {get; set;}
    [JsonProperty("speed")]
    public int? Speed {get; set;}
    [JsonProperty("category")]
    public string Category {get; set;} = string.Empty;
    [JsonProperty("messages")]
    public string Messages {get; set;} = string.Empty;
    [JsonProperty("rssi")] 
    public float? Rssi {get;set;}
}