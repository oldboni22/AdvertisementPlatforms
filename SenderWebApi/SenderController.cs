using Domain;
using Microsoft.AspNetCore.Mvc;
using Sender.Services.Abstractions;

namespace SenderWebApi;

[Route("sender")]
[ApiController]
public class SenderController(ISenderService senderService) : Controller
{
    private readonly ISenderService _senderService = senderService;
    
    [HttpPost]
    [RequestSizeLimit(500_000_000)]
    public async Task<IActionResult> SendDataAsync([FromBody] string serializedPlatforms)
    {
        await _senderService.SendPlatformListAsync(serializedPlatforms);
        return Ok();
    }
}