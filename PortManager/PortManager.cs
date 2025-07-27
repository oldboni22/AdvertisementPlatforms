using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using PortManager.Abstractions;
using Shared;

namespace PortManager;

public class PortManager(string address, string route,
    ILogger<PortManager>? logger = null) : IPortManager
{
    private readonly ILogger<PortManager>? _logger = logger;
    private readonly string _route = route;
    
    private readonly HttpClient _client = new()
    {
        BaseAddress = new Uri(address)
    };
    
    public async Task SendPlatformListAsync(string serializedUpdateRequestBody)
    {
        try
        {
            var response = await _client.PostAsJsonAsync(_route, serializedUpdateRequestBody);

            if (response.IsSuccessStatusCode)
            {
                
            }
            else
            {
                
            }
            
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex,"An exception occured while sending platform list.");
        }
    }

    public async ValueTask DisposeAsync()
    {
        await Task.Run(_client.Dispose);
    }
}