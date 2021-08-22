# MassTransit With Amazon SQS FIFO Queue
## Benefits of FIFO (First In, First Out)
- Strictly-preserved message ordering
- Exactly-once message delivery
- High throughput, up to 300 publishes/second
- Subscription protocols: SQS

## Configuration for FIFO
- Set Entity Name of the FIFO Message with postfix .fifo
- Set the Publish topic attributes

  ```cs
    x.UsingAmazonSqs((context, cfg) =>
    {
        ...

        cfg.Message<FifoMessage>(x =>
        {
            // required
            x.SetEntityName("message.fifo");
        });

        cfg.Publish<FifoMessage>(x =>
        {
            // required
            x.TopicAttributes["FifoTopic"] = "true";

            // optional
            x.TopicAttributes.Add("ContentBasedDeduplication", "true");
        });

        ...
    });
  ```

- Define custom Consumer Definition

  ```cs
    public class  FifoMessageConsumerDefinition 
        : ConsumerDefinition<FifoMessageConsumer>
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

        protected override void ConfigureConsumer(
            IReceiveEndpointConfigurator endpointConfigurator, 
            IConsumerConfigurator< FifoMessageConsumer> consumerConfigurator)
        {
            var sqsEndpointConfigurator = (IAmazonSqsReceiveEndpointConfigurator)endpointConfigurator;
            sqsEndpointConfigurator.QueueAttributes["FifoQueue"] = "true";

            // Enable for Content based Deduplication
            // sqsEndpointConfigurator.QueueAttributes["ContentBasedDeduplication"] = true;

            // Enable for Attribute based Deduplication
            // sqsEndpointConfigurator.QueueAttributes["MessageDeduplicationId"] = "DeduplicationId";
        }
    }
  ```
