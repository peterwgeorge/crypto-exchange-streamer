namespace WebsocketServer.ConnectionHandlers;

using System.Text;
using System.Net.WebSockets;
using Newtonsoft.Json;
using Credentials;
using Credentials.Types;
using CryptoExchangeModels.Common;
using CryptoExchangeModels.Common.Types;
using Normalization.Parse;
using Normalization.Types;

public class ConnectionHandler{
    
    public ClientWebSocket Socket;
    private readonly ExchangeConfig _config;
    private readonly ICredentialProvider _credentialProvider;

    public ConnectionHandler(ClientWebSocket socket, ExchangeConfig config)
    {
        Socket = socket;
        _config = config;
        if(_config.Authentication != null)
            _credentialProvider = CredentialProviderFactory.GetProvider(_config.Authentication.Provider);
    }
    
    public async Task Connect(){
        if (_config.Authentication != null)
        {
            _credentialProvider.Configure(_config.Authentication, ExchangeApiKeyTypeFactory.GetExchangeApiKeyType(_config));
            await _credentialProvider.Initialize();
        }

        string feed = FeedFactory.GetExchangeFeed(_config.Name);
        Uri uri = new Uri(feed);
        await Socket.ConnectAsync(uri, CancellationToken.None);
        Console.WriteLine($"Connected to {_config.Name} WebSocket feed {feed}.");
    }

    public async Task Subscribe()
    {
        IExchangeRequest request = RequestFactory.CreateSubscribeRequest(_config, _credentialProvider);
        string json = JsonConvert.SerializeObject(request);
        Console.WriteLine($"Sending {_config.Name} request message:");
        Console.WriteLine(json);

        var bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(json));
        await Socket.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public async Task Receive(byte[] buffer)
    {
        var result = await Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        if (result.MessageType == WebSocketMessageType.Close)
        {
            Console.WriteLine($"{_config.Name} WebSocket closed.");
            await Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }
        else
        {
            string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"Received from {_config.Name}: " + message);
            PricePoint p = ExchangeDataParser.GetPricePoint(message);
            if(p.Exchange == "unknown")
                return;

            await RelayServer.BroadcastToClientsAsync(JsonConvert.SerializeObject(p));
        }
    }
}