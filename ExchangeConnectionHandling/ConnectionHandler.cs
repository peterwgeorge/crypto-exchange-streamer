using Core.Interfaces;
using Core.Models;
using Credentials;
using Credentials.Types;
using CryptoExchangeModels.Common;
using CryptoExchangeModels.Common.Types;
using Newtonsoft.Json;
using Normalization.Parse;
using Normalization.Types;
using System.Net.WebSockets;
using System.Text;

namespace ExchangeConnectionHandling;
public class ConnectionHandler
{

    public ClientWebSocket Socket;
    private readonly ExchangeConfig _config;
    private readonly ICredentialProvider? _credentialProvider;
    private readonly IPriceEngine _priceEngine;

    private PeriodicTimer? _heartbeatTimer;
    private CancellationTokenSource? _heartbeatCts;
    private Task? _heartbeatTask;

    public ConnectionHandler(ClientWebSocket socket, ExchangeConfig config)
    {
        Socket = socket;
        _config = config;
        if (_config.Authentication != null)
            _credentialProvider = CredentialProviderFactory.GetProvider(_config.Authentication.Provider);
    }

    public ConnectionHandler(ClientWebSocket socket, ExchangeConfig config, IPriceEngine priceEngine)
    {
        Socket = socket;
        _priceEngine = priceEngine;
        _config = config;
        if (_config.Authentication != null)
            _credentialProvider = CredentialProviderFactory.GetProvider(_config.Authentication.Provider);
    }

    public async Task Connect()
    {
        if (_config.Authentication != null)
        {
            _credentialProvider.Configure(_config.Authentication, ExchangeApiKeyTypeFactory.GetExchangeApiKeyType(_config));
            await _credentialProvider.Initialize();
        }

        string feed = FeedFactory.GetExchangeFeed(_config.Name);
        Uri uri = new Uri(feed);
        await Socket.ConnectAsync(uri, CancellationToken.None);
        Console.WriteLine($"Connected to {_config.Name} WebSocket feed {feed}.");

        StartHeartbeatIfRequired();
    }

    public async Task Subscribe()
    {
        IExchangeRequest request = RequestFactory.CreateSubscribeRequest(_config, _credentialProvider);
        string json = JsonConvert.SerializeObject(request);
        Console.WriteLine($"Sending {_config.Name} request message:");
        Console.WriteLine(json);

        var bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(json));
        var token = _heartbeatCts == null ? CancellationToken.None : _heartbeatCts!.Token;
        await Socket.SendAsync(bytesToSend, WebSocketMessageType.Text, true, token);
    }

    public async Task Receive(byte[] buffer)
    {
        var result = await Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        if (result.MessageType == WebSocketMessageType.Close)
        {
            Console.WriteLine($"{_config.Name} WebSocket closed.");
            StopHeartbeat();
            await Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }
        else
        {
            string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"Received from {_config.Name}: " + message);
            PricePoint p = ExchangeDataParser.GetPricePoint(message);
            if (p.Exchange == "unknown")
                return;

            _priceEngine.Publish(new PriceUpdate(
                        _config.Name,
                        _config.Symbols[0],
                        Convert.ToDecimal(p.Price),
                        DateTime.UtcNow));
        }
    }

    private void StartHeartbeatIfRequired()
    {
        if (!_config.RequiresHeartbeat || _heartbeatTimer != null)
            return;

        _heartbeatCts = new CancellationTokenSource();
        var span = TimeSpan.FromSeconds(_config.HeartbeatIntervalSeconds);
        _heartbeatTimer = new PeriodicTimer(span);

        _heartbeatTask = RunHeartbeatAsync(_heartbeatCts.Token);
    }

    private async Task RunHeartbeatAsync(CancellationToken token)
    {
        try
        {
            while (await _heartbeatTimer!.WaitForNextTickAsync(token))
            {
                if (Socket.State != WebSocketState.Open)
                    break;

                await SendHeartbeatAsync();
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when stopping
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Heartbeat failure on {_config.Name}: {ex}");
        }
    }

    private async Task SendHeartbeatAsync()
    {
        var heartbeat =
            RequestFactory.CreateHeartbeatRequest(_config, _credentialProvider);

        var json = JsonConvert.SerializeObject(heartbeat);
        var bytes = Encoding.UTF8.GetBytes(json);

        await Socket.SendAsync(
            new ArraySegment<byte>(bytes),
            WebSocketMessageType.Text,
            true,
            _heartbeatCts!.Token);

        Console.WriteLine(
            $"[{DateTime.UtcNow:O}] Heartbeat sent to {_config.Name}");
    }
    
    private void StopHeartbeat()
    {
        _heartbeatCts?.Cancel();
        _heartbeatCts?.Dispose();
        _heartbeatCts = null;

        _heartbeatTimer?.Dispose();
        _heartbeatTimer = null;
    }
}