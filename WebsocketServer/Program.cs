using Core;
using Core.Interfaces;
using CryptoExchangeModels.Common.Types;

public class Program
{
    public static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var host = CreateHostBuilder(args, configuration).Build();

        await host.StartAsync();

        Console.WriteLine("DebugViewModel running. Press Ctrl+C to exit.");
        await Task.Delay(Timeout.Infinite);
    }

    public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                var exchangeConfigs =
                    configuration.GetSection("Exchanges").Get<List<ExchangeConfig>>();

                services.AddSingleton<IPriceEngine, PriceEngine>();

                foreach (var config in exchangeConfigs)
                {
                    services.AddSingleton<IHostedService>(provider =>
                        new ExchangeWebSocketService(
                            provider.GetRequiredService<ILogger<ExchangeWebSocketService>>(),
                            config,
                            provider.GetRequiredService<IPriceEngine>()
                        ));
                }
            });
    }
}
