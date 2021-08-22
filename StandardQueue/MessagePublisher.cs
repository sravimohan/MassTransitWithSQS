using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MassTransitWithSQS.StandardQueue
{
    public class MessagePublisher : BackgroundService
    {
        readonly IBus _bus;
        readonly ILogger<MessagePublisher> _logger;

        public MessagePublisher(IBus bus, ILogger<MessagePublisher> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var text = $"The time is {DateTimeOffset.Now}";
                _logger.LogInformation("Publishing Message: {text}", text);

                var message = new Message { Text = text };
                await _bus.Publish(message, stoppingToken);

                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
