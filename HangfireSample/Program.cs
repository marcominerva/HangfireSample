using Hangfire;
using Hangfire.SqlServer;
using HangfireSample.Hangfire;
using HangfireSample.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddSingleton<CpuUsageMonitor>();
builder.Services.AddSingleton<CpuAwareHangfireFilter>();

builder.Services.AddHangfire((serviceProvider, configuration) =>
    configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    })
    .UseFilter(serviceProvider.GetRequiredService<CpuAwareHangfireFilter>())
);

// Per gestire lo start e lo stop manuale, invece di usare AddHangfireServer, registriamo BackgroundJobServer come singleton e lo gestiamo tramite un hosted service.
builder.Services.AddSingleton(new BackgroundJobServerOptions
{
    ServerName = $"{builder.Environment.ApplicationName}:{Environment.MachineName}",
    WorkerCount = Math.Max(1, Environment.ProcessorCount / 2),
    Queues = ["default"],
    SchedulePollingInterval = TimeSpan.FromSeconds(15)
});
builder.Services.AddSingleton<HangfireServerManager>();
builder.Services.AddHostedService<HangfireServerHostedService>();

builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.UseHangfireDashboard("/jobs");

var recurringJobMananger = app.Services.GetRequiredService<IRecurringJobManager>();
recurringJobMananger.AddOrUpdate<IShoppingCartService>("cleanup", (service) => service.CleanupAsync(), Cron.Minutely);

app.MapControllers();

app.Run();
