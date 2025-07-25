using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Text.Json;
using AesService.Abstractions;
using Domain;
using Feed.Services.Abstractions;
using FeedData.Abstractions;
using Shared;

namespace Feed.Services;

public class FeedService(IFeedData feedData, IAesService aes) : IFeedService
{
    private readonly IAesService _aes = aes;
    private readonly IFeedData _feedData = feedData;
    
    public async Task WriteData(string serializedData)
    {
        var deserialized = await Task.Run(()=> JsonSerializer.Deserialize<UpdateRequestBody>(serializedData));
        
        var data = _aes.DecryptString
            (
                deserialized.EncryptedData, 
                Convert.FromBase64String(deserialized.Iv)
            );

        var platforms =  await Task.Run(() => JsonSerializer.Deserialize<IEnumerable<Platform>>(data));

        if (platforms == null)
        {
            
            return;
        }
        
        var dict = new ConcurrentDictionary<string, string[]>();
        
        var array = platforms.ToImmutableArray();
        
        await Task.Run(() => array.AsParallel().
            Select(platform => platform.Location).
            Distinct().
            ForAll(loc =>
            {
                var suitable = array.
                    Where(pl => loc.Contains(pl.Location)).
                    Select(pl => pl.Name).
                    ToArray();

                dict.TryAdd(loc, suitable);
            }
                ));

        var frozen = dict.ToFrozenDictionary();
        
        _feedData.WriteData(frozen);
    }
}