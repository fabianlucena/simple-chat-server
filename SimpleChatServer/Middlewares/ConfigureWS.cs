using SimpleChatServer.WSControllers;

namespace SimpleChatServer.Middlewares
{
    public static class WebApplicationExtension
    {
        static public void ConfigureWS(this WebApplication app)
        {
            app.UseWebSockets();

            app.Use(async (context, next) =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    if (context.Request.Path == "/ws/chat")
                    {
                        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await ChatWS.Handler(webSocket);
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
    }
}
