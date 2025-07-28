using System.Text.Json;
using AesService.Abstractions;
using Domain;
using Microsoft.Extensions.Logging;
using PortManager.Abstractions;
using Sender.Services.Abstractions;
using Shared;

namespace Sender.Services;

public class SenderService(IPortManager portManager, IAesService aes, 
    ILogger<SenderService>? logger = null) : ISenderService
{
    private readonly IAesService _aes = aes;
    private readonly ILogger<SenderService>? _logger = logger;
    private readonly IPortManager _portManager = portManager;
    
    public async Task SendPlatformListAsync(string serializedPlatforms)
    {
        try
        {
            var result = await _aes.EncryptStringAsync(serializedPlatforms);

            var body = new UpdateRequestBody
            (
                result.encrypted,
                result.iv
            );

            var serializedBody = JsonSerializer.Serialize(body);

            await _portManager.SendPlatformListAsync(serializedBody);

        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "An exception occured while sending a platform list.");
        }
    }
}