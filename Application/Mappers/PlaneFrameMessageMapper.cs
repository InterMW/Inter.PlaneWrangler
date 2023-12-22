using Domain;
using Application.Models;

namespace Application.Mappers;

public static class PlaneFrameMessageMapper
{
    public static PlaneFrame ToDomain(this PlaneFrameMessage message)
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