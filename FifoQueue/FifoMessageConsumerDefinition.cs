using MassTransit;
using MassTransit.AmazonSqsTransport;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;

namespace MassTransitWithSQS.FifoQueue
{
    public class  FifoMessageConsumerDefinition : ConsumerDefinition<FifoMessageConsumer>
    {
        public  FifoMessageConsumerDefinition()
        {
            Endpoint(x =>
            {
                x.Name = "message.fifo";
                x.ConcurrentMessageLimit = 1;
                x.PrefetchCount = 1;
            });
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator< FifoMessageConsumer> consumerConfigurator)
        {
            var sqsEndpointConfigurator = (IAmazonSqsReceiveEndpointConfigurator)endpointConfigurator;
            sqsEndpointConfigurator.QueueAttributes["FifoQueue"] = "true";

            // Enable for Content based Deduplication
            // sqsEndpointConfigurator.QueueAttributes["ContentBasedDeduplication"] = true;

            // Enable for Attribute based Deduplication
            // sqsEndpointConfigurator.QueueAttributes["MessageDeduplicationId"] = "DeduplicationId";
        }
    }
}
