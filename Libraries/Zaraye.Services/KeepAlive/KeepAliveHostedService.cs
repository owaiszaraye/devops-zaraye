using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;

namespace Zaraye.Services.KeepAlive
{
    public class KeepAliveHostedService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseUrl; // The base URL of your ASP.NET Core application
       
        public KeepAliveHostedService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(SendKeepAliveRequest, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
            return Task.CompletedTask;
        }

        private async void SendKeepAliveRequest(object state)
        {
            Console.WriteLine("KeepAlive");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
