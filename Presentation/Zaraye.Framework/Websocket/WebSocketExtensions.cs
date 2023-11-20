using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Zaraye.Framework.Websocket
{
   
    public static class WebSocketExtensions
    {
        public static IApplicationBuilder UseWebSocketServer(this IApplicationBuilder app)
        {
            return app.UseMiddleware<WebSocketServerMiddleware>();
        }

        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddSingleton<WebSocketServer>();

            return services;
        }
    }
}
