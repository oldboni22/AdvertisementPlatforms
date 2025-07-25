using System.Text.Json;
using AesService.Abstractions;
using Domain;
using Microsoft.Extensions.Logging;
using PortManager.Abstractions;
using Sender.Services.Abstractions;
using Shared;

namespace Sender.Services;

public class SenderServiceService(IPortManager portManager, IAesService aes, 
    ILogger? logger) : ISenderService
{
    private readonly IAesService _aes = aes;
    private readonly ILogger? _logger = logger;
    private readonly IPortManager _portManager = portManager;
    
    public async Task SendPlatformListAsync(IEnumerable<Platform> platforms)
    {
        try
        {
            var serialized = JsonSerializer.Serialize(platforms);
            var encrypted = _aes.EncryptString(serialized, out var iv);
            
            await _portManager.SendPlatformListAsync(
                new UpdateRequestBody
                    (
                        encrypted, 
                        Convert.ToBase64String(iv)
                    )
                );

        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "An exception occured while sending a platform list.");
        }
    }
}