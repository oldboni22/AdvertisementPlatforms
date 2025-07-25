using Domain;

namespace Sender.Abstractions;

public interface ISender
{
    Task SendPlatformListAsync(IEnumerable<Platform> platforms);
}