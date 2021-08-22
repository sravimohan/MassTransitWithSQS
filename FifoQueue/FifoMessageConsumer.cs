using MassTransit;
using MassTransitWithSQS.FifoQueue;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MassTransitWithSQS.FifoQueue
{
    public class FifoMessageConsumer : IConsumer<FifoMessage>
    {
        readonly ILogger<FifoMessageConsumer> _logger;

        public FifoMessageConsumer(ILogger<FifoMessageConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<FifoMessage> context)
        {
            _logger.LogInformation("Received FifoMessage: {Text}", context.Message.Text);

            return Task.CompletedTask;
        }
    }
}
