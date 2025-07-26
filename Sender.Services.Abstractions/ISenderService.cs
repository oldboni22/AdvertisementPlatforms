using Domain;

namespace Sender.Services.Abstractions;

public interface ISenderService 
{
    Task SendPlatformListAsync(string serializedPlatforms);
}