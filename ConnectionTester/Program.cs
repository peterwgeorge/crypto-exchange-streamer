using System.Net.WebSockets;
using WebsocketServer.ConnectionHandlers;

class Program
{  
    static async Task Main(string[] args)
    {
        using var socket = new ClientWebSocket();
        var handler = new ConnectionHandler(socket, "btcc");

        try{
            await handler.Connect();
            await handler.Subscribe("", ["BTCUSDT"]);
        
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