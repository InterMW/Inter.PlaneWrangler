namespace Infrastructure.Redis.Models;

public class PlaneFrameModel
{
    public long Now {get; set;}
    public PlaneModel[] Planes {get; set;} = Array.Empty<PlaneModel>();
}