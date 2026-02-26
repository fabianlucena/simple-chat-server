using SimpleChatServer.DTO;
using SimpleChatServer.Entities;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace SimpleChatServer.WSControllers
{
    public class ChatWS
    {
        private static readonly List<ChatClient> Clients = [];
        private static readonly List<ChatResponse> Messages = [];
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        static public async Task Handler(WebSocket webSocket, User user)
        {
            var client = new ChatClient(webSocket, user.Username);
            Clients.Add(client);

            Messages.ForEach(async message => await Send(webSocket, message));

            var buffer = new byte[10000];
            while (true)
            {
                var rcv = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (rcv.CloseStatus.HasValue)
                {
                    await webSocket.CloseAsync(rcv.CloseStatus.Value, rcv.CloseStatusDescription, CancellationToken.None);
                    break;
                }

                var msg = Encoding.UTF8.GetString(buffer, 0, rcv.Count);
                ChatRequest? req;

                try
                {
                    req = JsonSerializer.Deserialize<ChatRequest>(msg, JsonOptions);
                }
                catch (Exception)
                {
                    req = new ChatRequest {
                        Command = "message",
                        Message = msg,
                    };
                }

                if (req is null)
                {
                    await Send(
                        client.WebSocket,
                        new ChatResponse
                        {
                            Type = "error",
                            Message = "Invalid request",
                        }
                    );

                    continue;
                }

                if (string.IsNullOrEmpty(req.Command))
                {
                    await Send(
                        client.WebSocket,
                        new ChatResponse
                        {
                            Type = "error",
                            Message = "No command property in JSON request",
                        }
                    );

                    continue;
                }

                var command = req.Command.ToLowerInvariant();
                if (command == "message")
                {
                    await AddMessage(
                        new ChatResponse
                        {
                            Type = "message",
                            User = client.User,
                            Message = req.Message,
                        },
                        client
                    );
                    continue;
                }

                if (command == "user")
                {
                    await AddMessage(
                        new ChatResponse
                        {
                            Type = "user",
                            User = client.User,
                            Message = $"User {client.User} changed name to {req.Message}",
                        },
                        client
                    );
                    client .User = req.Message;
                    continue;
                }

                await Send(
                    client.WebSocket,
                    new ChatResponse
                    {
                        Type = "error",
                        Message = $"Unknown request command: {req.Command}",
                    }
                );
            }
            Clients.Remove(client);
        }

        static public async Task Send(WebSocket webSocket, ChatResponse item)
        {
            var response = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(item, JsonOptions));
            await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        static public async Task AddMessage(ChatResponse message, ChatClient skipClient)
        {
            Messages.Add(message);
            Clients.ForEach(async client =>
             {
                 if (client != skipClient)
                 {
                     await Send(client.WebSocket, message);
                 }
             });
        }
    }
}
