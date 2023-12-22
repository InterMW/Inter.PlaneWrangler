using Domain;
using Application.Models;

namespace Application.Mappers;

public static class PlaneMapper
{
    public static TimeAnotatedPlane ToDomain(this AirplaneData data)
    {
       if(data == null) return null;
       return new TimeAnotatedPlane
       {
            Altitude = data.altitude,
            AltitudeUpdated = data.altitude_update,
            Category = data.category,
            CategoryUpdated = data.category_update,
            Flight = data.flight,
            FlightUpdated = data.flight_update,
            HexValue = data.hex,
            Latitude = data.lat,
            Longitude = data.lon,
            PositionUpdated = data.position_update,
            Messages = data.messages,
            Nucp = data.nucp,
            Rssi = data.rssi,
            Speed = data.speed,
            SpeedUpdated = data.speed_update,
            Squawk = data.squawk,
            SquawkUpdated = data.squawk_update,
            Track = data.track,
            TrackUpdated = data.track_update,
            VerticleRate = data.vert_rate,
            VerticleRateUpdated = data.vert_update
       };
    }
}