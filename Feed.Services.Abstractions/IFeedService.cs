using Shared;

namespace Feed.Services.Abstractions;

public interface IFeedService
{
    Task WriteData(string serializedData);
    Task<GetResultBody>? GetPlatforms(string query);
}