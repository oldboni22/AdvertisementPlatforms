using System.Text.Json;
using AesService.Abstractions;
using Domain;
using Microsoft.Extensions.Logging;
using PortManager.Abstractions;
using Sender.Services.Abstractions;
using Shared;

namespace Sender.Services;

public class SenderService(IPortManager portManager, IAesService aes, 
    ILogger<SenderService>? logger) : ISenderService
{
    private readonly IAesService _aes = aes;
    private readonly ILogger<SenderService>? _logger = logger;
    private readonly IPortManager _portManager = portManager;
    
    public async Task SendPlatformListAsync(IEnumerable<Platform> platforms)
    {
        var asList = platforms.ToList();
        
        if(asList.Count == 0)
            return;
        
        try
        {
            var serialized = JsonSerializer.Serialize(asList);
            var result = await _aes.EncryptStringAsync(serialized);
            
            await _portManager.SendPlatformListAsync(
                new UpdateRequestBody
                    (
                        result.encrypted, 
                        result.iv
                    )
                );

        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "An exception occured while sending a platform list.");
        }
    }
}