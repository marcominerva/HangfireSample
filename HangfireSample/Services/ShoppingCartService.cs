namespace HangfireSample.Services;

public class ShoppingCartService : IShoppingCartService
{
    private readonly ILogger<ShoppingCartService> logger;

    public ShoppingCartService(ILogger<ShoppingCartService> logger)
    {
        this.logger = logger;
    }

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
