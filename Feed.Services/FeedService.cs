using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Text.Json;
using AesService.Abstractions;
using Domain;
using Feed.Services.Abstractions;
using FeedData.Abstractions;
using Microsoft.Extensions.Logging;
using Shared;

namespace Feed.Services;

public class FeedService(IFeedData feedData, IAesService aes,
    ILogger<FeedService>? logger = null) : IFeedService
{
    private readonly IAesService _aes = aes;
    private readonly IFeedData _feedData = feedData;
    private readonly ILogger<FeedService>? _logger = logger;

    private ConcurrentDictionary<string, GetResultBody> _cached = new();
    
    public async Task WriteData(string serializedData)
    {
        try
        {
            if (string.IsNullOrEmpty(serializedData))
            {
                throw new ArgumentNullException();
            }

            var deserialized = await Task.Run(() => JsonSerializer.Deserialize<UpdateRequestBody>(serializedData));

            var data = await _aes.DecryptStringAsync
            (
                deserialized.EncryptedData,
                deserialized.Iv
            );

            var platforms = await Task.Run(() => JsonSerializer.Deserialize<IEnumerable<Platform>>(data));

            if (platforms == null)
                throw new ArgumentNullException();
            
            var array = platforms.ToImmutableArray();
            
            if (array.Length == 0)
                throw new ArgumentNullException();

            var dict = new ConcurrentDictionary<string, string[]>();
            
            array.
                AsParallel().
                Select(platform => platform.Location).
                Distinct().
                ForAll(loc =>
                {
                    var suitable = array.Where(pl => loc.Contains(pl.Location)).Select(pl => pl.Name).ToArray();

                    dict.TryAdd(loc, suitable);
                }
            );

            var frozen = dict.ToFrozenDictionary();

            _cached = new();
            _feedData.WriteData(frozen);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "An error occured while writing data.");
            throw;
        }
    }

    public async Task<GetResultBody>? GetPlatforms(string query)
    {
        if (string.IsNullOrEmpty(query))
            return default;
        
        try
        {
            var result = _feedData.GetPlatforms(query);

            if (result == null)
                return default;

            if (_cached.TryGetValue(query, out var value) is false)
            {
                var serialized = await Task.Run(() => JsonSerializer.Serialize(result));
                var output = await _aes.EncryptStringAsync(serialized);

                value = new GetResultBody(output.encrypted, output.iv);
            }
            
            return value;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex,"An exception occured while fetching platforms.");
        }

        return default;
    }
}