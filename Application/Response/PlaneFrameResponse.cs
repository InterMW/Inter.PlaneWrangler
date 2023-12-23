using Newtonsoft.Json;

namespace Application.Responses;
public class PlaneFrameResponse
{

    [JsonProperty("now")] 
    public long Now {get; set;}
    [JsonProperty("planes")] 
    public PlaneResponse[] Planes {get; set;}
    [JsonProperty("antenna")] 
    public string Antenna {get; set;}
    [JsonProperty("source")] 
    public string Source {get; set;}
}