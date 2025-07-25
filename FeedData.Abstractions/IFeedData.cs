using System.Collections.Frozen;
using Domain;

namespace FeedData.Abstractions;

public interface IFeedData
{
    void WriteData(FrozenDictionary<string, string[]> dictionary);
    IEnumerable<string>? GetPlatforms(string location);
}