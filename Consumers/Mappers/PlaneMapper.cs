using Inter.Domain;
using Inter.Consumers.Models;

namespace Inter.Consumers.Mappers;
public static class PlaneMapper
{
    public static TimeAnotatedPlane ToDomain(this AirplaneData data)
    {
       return new TimeAnotatedPlane
       {
            Altitude = data?.altitude,
            AltitudeUpdated = data?.altitude_update,
            Category = data?.category ?? string.Empty,
            CategoryUpdated = data?.category_update,
            Flight = data?.flight ?? string.Empty,
            FlightUpdated = data?.flight_update,
            HexValue = data?.hex ?? string.Empty,
            Latitude = data?.lat ?? null,
            Longitude = data?.lon ?? null,
            PositionUpdated = data?.position_update,
            Messages = data?.messages ?? string.Empty,
            Nucp = data?.nucp ?? string.Empty,
            Rssi = data?.rssi,
            Speed = data?.speed,
            SpeedUpdated = data?.speed_update,
            Squawk = data?.squawk ?? string.Empty,
            SquawkUpdated = data?.squawk_update,
            Track = data?.track ?? null,
            TrackUpdated = data?.track_update,
            VerticleRate = data?.vert_rate ?? null,
            VerticleRateUpdated = data?.vert_update
       };
    }
}