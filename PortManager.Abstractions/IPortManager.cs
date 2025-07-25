using Domain;
using Shared;

namespace PortManager.Abstractions;

public interface IPortManager
{
    Task SendPlatformListAsync(UpdateRequestBody body);
}