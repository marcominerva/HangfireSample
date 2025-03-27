namespace HangfireSample.Services;

public class ShoppingCartService(ILogger<ShoppingCartService> logger) : IShoppingCartService
{
    public async Task CheckoutAsync()
    {
        logger.LogInformation("Starting checkout...");

        await Task.Delay(5000);

        //throw new Exception("Incredible Exception!");

        logger.LogInformation("Checkout completed");
    }

    public async Task CleanupAsync()
    {
        logger.LogInformation("Starting carts cleanup...");

        await Task.Delay(5000);

        logger.LogInformation("Cleanup completed");

    }
}
