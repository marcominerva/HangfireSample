using Hangfire;

namespace HangfireSample.Services;

public interface IShoppingCartService
{
    [AutomaticRetry(Attempts = 0)]
    Task CheckoutAsync(CancellationToken cancellationToken);

    Task CleanupAsync();
}