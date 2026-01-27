using Core;
using Core.Interfaces;
using CryptoExchangeModels.Common.Types;
using Desktop.ViewModels;
using ExchangeConnectionHandling.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace Desktop;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public IHost Host { get; private set; } = null!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            .ConfigureServices((ctx, services) =>
            {
                services.AddSingleton<IPriceEngine, PriceEngine>();
                services.AddSingleton<MainViewModel>();

                var exchangeConfigs = configuration
                    .GetSection("Exchanges")
                    .Get<List<ExchangeConfig>>();

                foreach (var config in exchangeConfigs)
                {
                    services.AddSingleton<IHostedService>(sp =>
                        new ExchangeWebSocketService(
                            sp.GetRequiredService<ILogger<ExchangeWebSocketService>>(),
                            config,
                            sp.GetRequiredService<IPriceEngine>()
                        ));
                }
            })
            .Build();

        await Host.StartAsync();

        var window = new MainWindow();
        window.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (Host != null)
            await Host.StopAsync();

        base.OnExit(e);
    }
}


