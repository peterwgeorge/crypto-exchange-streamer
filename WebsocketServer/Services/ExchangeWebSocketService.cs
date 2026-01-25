using WebsocketServer.ConnectionHandlers;
using CryptoExchangeModels.Common.Types;
using System.Net.WebSockets;

public class ExchangeWebSocketService : BackgroundService
{
    private readonly ILogger<ExchangeWebSocketService> _logger;
    private readonly ExchangeConfig _config;

    public ExchangeWebSocketService(
        ILogger<ExchangeWebSocketService> logger,
        ExchangeConfig config)
    {
        _logger = logger;
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"Starting {_config.Name} WebSocket service");
        using var socket = new ClientWebSocket();
        var handler = new ConnectionHandler(socket, _config);

        try
        {
            await handler.Connect();
            await handler.Subscribe();

            byte[] buffer = new byte[4096];

            while (!stoppingToken.IsCancellationRequested && socket.State == WebSocketState.Open)
            {
                await handler.Receive(buffer);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An unexpected error occurred in the {_config.Name} WebSocket service");
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
                    _logger.LogWarning(ex, $"Error closing {_config.Name} WebSocket connection");
                }
            }
        }
    }
}

