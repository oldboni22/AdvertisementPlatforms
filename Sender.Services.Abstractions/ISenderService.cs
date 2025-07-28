using Domain;

namespace Sender.Services.Abstractions;

public interface ISenderService 
{
    #region Description
    /// <summary>
    /// Sends update request to the feed service.
    /// </summary>
    /// <param name="serializedPlatforms">Serialized list of platforms</param>
    /// <returns></returns>
    #endregion
    Task SendPlatformListAsync(string serializedPlatforms);
}