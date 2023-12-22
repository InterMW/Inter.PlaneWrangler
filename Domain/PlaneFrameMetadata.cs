namespace Domain;
public class PlaneFrameMetadata
{
    public string Hostname {get; set;} = string.Empty;
    public string Antenna {get; set;} = string.Empty;
    public int Detailed {get; set;}
    public int Total {get; set;}
    public DateTime Timestamp {get; set;}
}