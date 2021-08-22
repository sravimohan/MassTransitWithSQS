using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.AmazonSqsTransport;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MassTransitWithSQS.FifoQueue
{
    public class FifoMessagePublisher : BackgroundService
    {
        readonly IBus _bus;
        readonly ILogger<FifoMessagePublisher> _logger;

        public FifoMessagePublisher(IBus bus, ILogger<FifoMessagePublisher> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var text = $"The time is {DateTimeOffset.Now}";
                _logger.LogInformation("Publishing FifoMessage: {text}", text);

                var groupId = Guid.NewGuid().ToString();
                var deduplicationId = Guid.NewGuid().ToString();
                var fifoMessage = new FifoMessage { Text = text };

                await _bus.Publish(fifoMessage, x => { x.SetGroupId(groupId); x.SetDeduplicationId(deduplicationId); }, stoppingToken);

                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
