namespace Feed.Services.Abstractions;

public interface IFeedService
{
    Task WriteData(string serializedData);
}