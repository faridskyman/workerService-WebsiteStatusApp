using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebsiteStatusApp
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private HttpClient client;
        private readonly WorkerOptions _options;


        /// <summary>
        /// init with 2 param as in program, 2 was sent via DI
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="options"></param>
        public Worker(ILogger<Worker> logger, WorkerOptions options)
        {
            _logger = logger;
            _options = options;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            client = new HttpClient();
            _logger.LogInformation("the service has started");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            client.Dispose();
            _logger.LogInformation("the service is stopping");
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //get url via config
                var result = await client.GetAsync(_options.SiteURI);

                if (result.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Website is up, status code: '{result.StatusCode}'");
                    //_logger.LogInformation($"the website is up, Headers code: '{result.Headers}'");
                }
                else
                {
                    _logger.LogError($"Website down, status code: '{result.StatusCode}'");
                }
                
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
