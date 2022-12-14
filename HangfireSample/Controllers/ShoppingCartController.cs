using System.Net.Mime;
using Hangfire;
using HangfireSample.Services;
using Microsoft.AspNetCore.Mvc;

namespace HangfireSample.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class ShoppingCartController : ControllerBase
{
    private readonly IBackgroundJobClient backgroundJobClient;
    private readonly IRecurringJobManager recurringJobClient;

    public ShoppingCartController(IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobClient)
    {
        this.backgroundJobClient = backgroundJobClient;
        this.recurringJobClient = recurringJobClient;
    }

    [HttpPost("enqueue-checkout")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public IActionResult EnqueueCheckout()
    {
        var jobId = backgroundJobClient.Enqueue<IShoppingCartService>((service) => service.CheckoutAsync());
        Console.WriteLine($"Created JobId: {jobId}");

        return Accepted();
    }

    [HttpPost("schedule-checkout")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public IActionResult ScheduleCheckout()
    {
        var jobId = backgroundJobClient.Schedule<IShoppingCartService>((service) => service.CheckoutAsync(), DateTimeOffset.UtcNow.AddHours(1));
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
