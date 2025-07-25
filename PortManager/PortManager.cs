using System.Net.Http.Json;
using PortManager.Abstractions;
using Shared;

namespace PortManager;

public class PortManager(string address, string route) : IPortManager
{
    private readonly string _route = route;
    private readonly HttpClient _client = new()
    {
        BaseAddress = new Uri(address)
    };
    
    public async Task SendPlatformListAsync(UpdateRequestBody body)
    {
        await _client.PostAsJsonAsync(_route, body);
    }

    public async ValueTask DisposeAsync()
    {
        await Task.Run(_client.Dispose);
    }
}