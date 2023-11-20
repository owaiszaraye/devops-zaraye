using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
namespace Zaraye.Framework.Websocket
{
   

    public class WebSocketServerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly WebSocketServer _webSocketServer;

        public WebSocketServerMiddleware(RequestDelegate next, WebSocketServer webSocketServer)
        {
            _next = next;
            _webSocketServer = webSocketServer;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == "/ws")
            {
                await _webSocketServer.HandleWebSocketRequest(context);
            }
            else
            {
                await _next(context);
            }
        }
    }
}
