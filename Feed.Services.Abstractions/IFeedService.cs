using Shared;

namespace Feed.Services.Abstractions;


public interface IFeedService
{
    #region description
    ///<summary>Replaces the data of feed with given set. Clears the cached responses.</summary>>
    /// <param name="serializedData">Serialized update request body.</param>
    #endregion
    Task WriteData(string serializedData);
    
    
    #region descriotion
    /// <returns>Result body, which is serialized list of strings, and an initialization vector.
    /// If no data matches the query, returns default.</returns>
    #endregion
    Task<GetResultBody> GetPlatforms(string query);
}