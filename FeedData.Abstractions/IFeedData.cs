using System.Collections.Frozen;
using Domain;

namespace FeedData.Abstractions;

public interface IFeedData
{
    #region Description
    /// <summary>
    /// Replaces the data with given dictionary.
    /// </summary>
    #endregion
    void WriteData(FrozenDictionary<string, string[]> dictionary);
    IEnumerable<string>? GetPlatforms(string location);
}