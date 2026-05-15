using System.Net.Mime;
using HangfireSample.Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace HangfireSample.Controllers;

[ApiController]
[Route("api/hangfire-server")]
[Produces(MediaTypeNames.Application.Json)]
public class HangfireServerController(HangfireServerManager serverManager) : ControllerBase
{
    [HttpGet("status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetStatus()
    {
        return Ok(new { isRunning = serverManager.IsRunning });
    }

    [HttpPost("start")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Start()
    {
        serverManager.Start();

        return NoContent();
    }

    [HttpPost("stop")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Stop()
    {
        serverManager.Stop();

        return NoContent();
    }
}
