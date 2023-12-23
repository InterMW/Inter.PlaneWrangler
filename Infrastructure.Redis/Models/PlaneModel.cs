namespace Infrastructure.Redis;

public class PlaneModel
{
    public string hexValue {get; set;} = string.Empty;
    public ulong? squawk_update {get; set;}
    public ulong? flight_update {get; set;}
    public ulong? position_update {get; set;}
    public ulong? altitude_update {get; set;}
    public ulong? vert_update {get; set;}
    public ulong? track_update {get; set;}
    public ulong? speed_update {get; set;}
    public ulong? category_update {get; set;}
    public string squawk {get; set;} = string.Empty;
    public string flight {get; set;} = string.Empty;
    public float? lat {get; set;}
    public float? lon {get; set;}
    public string nucp {get; set;} = string.Empty;
    public int? altitude {get; set;}
    public int? vert_rate {get; set;}
    public int? track {get; set;}
    public int? speed {get; set;}
    public string category {get; set;} = string.Empty;
    public string messages {get; set;} = string.Empty;
    public float? rssi {get;set;}
}