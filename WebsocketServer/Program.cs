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
        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<RelayServer>();
                webBuilder.UseUrls(configuration["RelayServerUrl"]);
            })
            .ConfigureServices((hostContext, services) =>
            {
                var exchangeConfigs = configuration.GetSection("Exchanges").Get<List<ExchangeConfig>>();

                foreach (var config in exchangeConfigs)
                {
                    services.AddSingleton<IHostedService>(provider =>
                        new ExchangeWebSocketService(
                            provider.GetRequiredService<ILogger<ExchangeWebSocketService>>(),
                            config
                        ));
                }
            });
    }
}
