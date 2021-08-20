using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace iEnvironment.Watchman
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private MessageProcessor Processor;
        private readonly WorkerOptions options;
        public Worker(ILogger<Worker> logger, WorkerOptions options)
        {
            Processor = new MessageProcessor(options.MqttEndpoint, options.MqttPort);
            _logger = logger;
            this.options = options;
            Processor.Start();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!Processor.Status) Processor.Start(); 
               
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
