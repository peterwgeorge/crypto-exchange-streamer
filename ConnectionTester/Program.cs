using System.Net.WebSockets;
using WebsocketServer.ConnectionHandlers;
using CryptoExchangeModels.Common.Types;

class Program
{  
    static async Task Main(string[] args)
    {
        
        using var socket = new ClientWebSocket();
        var config = new ExchangeConfig(){ Channel = "ticker", Name = "kraken", Symbols = ["BTC/USDT"]};
        var handler = new ConnectionHandler(socket, config);

        try{
            await handler.Connect();
            await handler.Subscribe();
        
            byte[] buffer = new byte[4096];
            while (socket.State == WebSocketState.Open)
            {
                await handler.Receive(buffer);
            }
        }
        catch (WebSocketException ex){
                Console.WriteLine("WebSocket error: " + ex.Message);
        }
        
    }
}