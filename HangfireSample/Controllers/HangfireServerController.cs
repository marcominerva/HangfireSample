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
    public async Task<IActionResult> StartAsync(CancellationToken cancellationToken)
    {
        await serverManager.StartAsync(cancellationToken);

        return NoContent();
    }

    [HttpPost("stop")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> StopAsync(CancellationToken cancellationToken)
    {
        await serverManager.StopAsync(cancellationToken);

        return NoContent();
    }
}
