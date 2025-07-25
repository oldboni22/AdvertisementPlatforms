using System.Net.Http.Json;
using Domain;
using PortManager.Abstractions;
using Shared;

namespace PortManager;

public class PortManager(string address) : IPortManager
{
    private readonly HttpClient _client = new()
    {
        BaseAddress = new Uri(address)
    };
    
    public async Task SendPlatformListAsync(UpdateRequestBody body)
    {
        await _client.PostAsJsonAsync("feed", body);
    }

    public async ValueTask DisposeAsync()
    {
        await Task.Run(_client.Dispose);
    }
}