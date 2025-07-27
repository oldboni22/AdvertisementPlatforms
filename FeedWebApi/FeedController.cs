using Domain;
using Feed.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace FeedWebApi;


[Route("feed")]
[ApiController]
public class FeedController(IFeedService feedService) : Controller
{
    private readonly IFeedService _feedService = feedService;
    
    [HttpPost]
    public async Task<IActionResult> WriteDataAsync([FromBody] string serializedData)
    {
        await _feedService.WriteData(serializedData);
        
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetDataAsync([FromQuery] string query)
    {
        var data = await _feedService.GetPlatforms(query)!; 
        
        if (string.IsNullOrWhiteSpace(data.EncryptedData))
            return NoContent();

        return Ok(data);
    }
}