using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MassTransitWithSQS
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
                _logger.LogInformation("Publishing Text: {text}", text);

                await _bus.Publish(new Message { Text = text }, stoppingToken);

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
