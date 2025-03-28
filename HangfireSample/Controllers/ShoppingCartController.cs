using System.Net.Mime;
using Hangfire;
using HangfireSample.Services;
using Microsoft.AspNetCore.Mvc;

namespace HangfireSample.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class ShoppingCartController(IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobClient) : ControllerBase
{
    [HttpPost("enqueue-checkout")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public IActionResult EnqueueCheckout()
    {
        var jobId = backgroundJobClient.Enqueue<IShoppingCartService>((service) => service.CheckoutAsync(CancellationToken.None));
        Console.WriteLine($"Created JobId: {jobId}");

        return Accepted(new { jobId });
    }

    [HttpDelete("delete-checkout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult DeleteCheckout(string jobId)
    {
        backgroundJobClient.Delete(jobId);

        return NoContent();
    }

    [HttpPost("schedule-checkout")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public IActionResult ScheduleCheckout()
    {
        var jobId = backgroundJobClient.Schedule<IShoppingCartService>((service) => service.CheckoutAsync(CancellationToken.None), DateTimeOffset.UtcNow.AddHours(1));
        Console.WriteLine($"Created JobId: {jobId}");

        return Accepted();
    }

    [HttpPost("recurring-change-schedule")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult ChangeSchedule(string cronExpression)
    {
        recurringJobClient.AddOrUpdate<IShoppingCartService>("cleanup", (service) => service.CleanupAsync(), cronExpression);

        return NoContent();
    }
}
