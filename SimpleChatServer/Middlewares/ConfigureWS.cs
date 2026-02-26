using SimpleChatServer.Entities;
using SimpleChatServer.Services;
using SimpleChatServer.WSControllers;

namespace SimpleChatServer.Middlewares
{
    public static class WebApplicationExtension
    {
        private static readonly AuthService authService = new();

        static public void ConfigureWS(this WebApplication app)
        {
            app.UseWebSockets();

            app.Use(async (context, next) =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    if (context.Request.Path == "/ws/chat")
                    {
                        var user = CheckCredentials(context);
                        if (user == null)
                        {
                            context.Response.StatusCode = 401;
                            return;
                        }

                        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await ChatWS.Handler(webSocket, user);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }
                
            });
        }

        static private User? CheckCredentials(HttpContext context)
        {
            var requested = context.WebSockets.WebSocketRequestedProtocols;
            var authIndex = requested.IndexOf("auth");
            if (authIndex < 0)
                return null;

            authIndex++;
            if (authIndex >= requested.Count)
                return null;

            string? authToken = requested[authIndex];
            if (authToken == null || !authToken.StartsWith("Bearer-"))
                return null;

            var token = authToken.Substring("Bearer-".Length).Trim();
            if (String.IsNullOrEmpty(token))
                return null;

            var user = authService.GetUserByToken(token);

            return user;
        }
    }
}
