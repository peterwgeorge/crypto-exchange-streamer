using AmazonSecretsManagerHandler;
using CryptoExchangeModels.Coinbase;
using WebsocketServer.ConnectionHandlers;
using System.Net.WebSockets;

public class ExchangeWebSocketService : BackgroundService
{
    private readonly ILogger<ExchangeWebSocketService> _logger;
    private readonly IConfiguration _configuration;
    private readonly ExchangeConfig _exchangeConfig;
    private readonly string _name;
    private readonly string _channel;
    private readonly string[] _symbols;

    public ExchangeWebSocketService(
        ILogger<ExchangeWebSocketService> logger,
        IConfiguration configuration,
        ExchangeConfig exchangeConfig)
    {
        _logger = logger;
        _configuration = configuration;
        _exchangeConfig = exchangeConfig;
        _name = exchangeConfig.Name;
        _channel = exchangeConfig.Channel;
        _symbols = exchangeConfig.Symbols.ToArray();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"Starting {_name} WebSocket service");
        using var socket = new ClientWebSocket();
        var handler = new ConnectionHandler(socket, _name);

        try
        {
            if (_exchangeConfig.Authentication != null)
            {
                SecretsProvider.Configure(_exchangeConfig.Name, _exchangeConfig.Authentication);
                await SecretsProvider.Initialize(_exchangeConfig.Name);
            }

            
            await handler.Connect();
            await handler.Subscribe(_channel, _symbols);

            byte[] buffer = new byte[4096];

            while (!stoppingToken.IsCancellationRequested && socket.State == WebSocketState.Open)
            {
                await handler.Receive(buffer);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An unexpected error occurred in the {_name} WebSocket service");
        }
        finally
        {
            if (socket.State == WebSocketState.Open)
            {
                try
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Service stopping", stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Error closing {_name} WebSocket connection");
                }
            }
        }
    }
}

