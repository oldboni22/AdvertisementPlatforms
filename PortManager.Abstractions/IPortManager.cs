using Domain;
using Shared;

namespace PortManager.Abstractions;

public interface IPortManager : IAsyncDisposable
{
    Task SendPlatformListAsync(string serializedUpdateRequestBody);
}