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
    public async Task<IActionResult> SendDataAsync([FromBody] List<Platform> platforms)
    {
        await _senderService.SendPlatformListAsync(platforms);
        return Ok();
    }
}