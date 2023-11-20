using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zaraye.Framework.Websocket
{
    public class WebSocketServer
    {
        private readonly ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        public async Task HandleWebSocketRequest(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var socket = await context.WebSockets.AcceptWebSocketAsync();
                var socketId = Guid.NewGuid().ToString();

                _sockets.TryAdd(socketId, socket);

                await Receive(socket, async (result, buffer) =>
                {
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        // Handle client messages if needed
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        WebSocket ignoredSocket;
                        _sockets.TryRemove(socketId, out ignoredSocket);
                    }
                });
            }
            //else
            //{
            //    context.Response.StatusCode = 400;
            //}
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                handleMessage(result, buffer);
            }
        }

        public void NotifyAllClients(string message)
        {
            foreach (var socket in _sockets)
            {
                if (socket.Value.State == WebSocketState.Open)
                {
                    var encodedMessage = Encoding.UTF8.GetBytes(message);
                    var buffer = new ArraySegment<byte>(encodedMessage, 0, encodedMessage.Length);
                    socket.Value.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}
