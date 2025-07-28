using Domain;
using Shared;

namespace PortManager.Abstractions;

public interface IPortManager : IAsyncDisposable
{
    #region Description
    /// <summary>
    /// Sends body to the feed service via http.
    /// </summary>
    /// <param name="serializedData">Serialized update body.</param>
    #endregion
    Task SendPlatformListAsync(string serializedData);
}