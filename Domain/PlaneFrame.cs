namespace Domain;

public class PlaneFrame
{
    public long Now {get; set;} = 0;
    public TimeAnotatedPlane[] Planes {get; set;} = Array.Empty<TimeAnotatedPlane>();
    public string Antenna {get; set;} = string.Empty;
    public string Source {get; set;} = string.Empty;
}
