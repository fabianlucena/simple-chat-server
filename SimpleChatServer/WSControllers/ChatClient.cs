using System.Net.WebSockets;

namespace SimpleChatServer.WSControllers
{
    public class ChatClient(
        WebSocket webSocket,
        string user
    )
    {
        public WebSocket WebSocket { get; set; } = webSocket;
        public string User { get; set; } = user;
    }
}
