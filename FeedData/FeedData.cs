using System.Collections.Concurrent;
using System.Collections.Frozen;
using FeedData.Abstractions;
using Microsoft.Extensions.Logging;

namespace FeedData;

public class FeedData(ILogger<FeedData> logger) : IFeedData
{
    private ConcurrentDictionary<string, IEnumerable<string>> _cached = new();
    
    private readonly ILogger<FeedData>? _logger = logger;
    
    private readonly ReaderWriterLockSlim _lock = new();
    private FrozenDictionary<string, string[]> _dictionary = FrozenDictionary<string, string[]>.Empty;
    
    public void WriteData(FrozenDictionary<string, string[]> dictionary)
    {
        try
        {
            _lock.EnterWriteLock();
            
            _cached = new();
            _dictionary = dictionary;
        }
        catch (Exception e)
        {
            _logger?.LogError(e,"An exception occured while writing data.");
            throw;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public IEnumerable<string>? GetPlatforms(string location)
    {
        try
        {
            _lock.EnterReadLock();
            
            if(_cached.TryGetValue(location, out var value) is false)
            {
                value = _dictionary.GetValueOrDefault(location);
                if(value != null)
                    _cached.TryAdd(location, value);
            }
            
            return value?.ToFrozenSet();
        }
        catch (Exception e)
        {
            _logger?.LogError(e,"An exception occured while writing data.");
            throw;
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }
}