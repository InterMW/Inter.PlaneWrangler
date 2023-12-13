using Inter.Consumers.Models;
using Inter.Domain;

namespace Inter.Consumers.Mappers;
public static class PlaneIngestionMessageMapper
{
    public static PlaneFrame ToDomain(this AirplaneRecord message)
    {
        if(message == null)
        {
            return null;
        }

        return new PlaneFrame
        {
            Antenna = message.Antenna,
            Now = (long)message.Now,
            Planes = message.Planes.Select(_ => _.ToDomain()).ToArray(),
            Source = message.Source
        };
    }
}