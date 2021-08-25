using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MassTransit;
using MassTransitWithSQS.FifoQueue;
using MassTransitWithSQS.StandardQueue;

namespace MassTransitWithSQS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    _ = services.AddMassTransit(x =>
                      {
                          x.AddConsumers(typeof(Program).Assembly);

                          x.UsingAmazonSqs((context, cfg) =>
                          {
                              const string awsRegion = "ap-southeast-2";
                              cfg.Host(awsRegion, h =>
                              {
                                // optional - set the AWS Access key and Secrect here if its not configured in the environment
                                // h.AccessKey("your-iam-access-key");
                                // h.SecretKey("your-iam-secret-key");

                                // optional - specify a scope for all queues
                                // h.Scope("dev");

                                // optional - scope topics as well
                                // h.EnableScopedTopics();
                              });

                              //optional - override the entity name for standard Topic Name
                              cfg.Message<Message>(x =>
                              {
                                  x.SetEntityName("message");
                              });

                              //required - override the entity name with .fifo postfix for FIFO Topic Name
                              cfg.Message<FifoMessage>(x =>
                              {
                                  x.SetEntityName("message.fifo");
                              });

                              
                              cfg.Publish<FifoMessage>(x =>
                              {
                                  //required - set FIFO Topic attributes
                                  x.TopicAttributes["FifoTopic"] = "true";

                                  //optional Content Based Deduplication flag
                                  x.TopicAttributes.Add("ContentBasedDeduplication", "true");
                              });

                              cfg.ConfigureEndpoints(context);
                          });
                      });

                    services.AddMassTransitHostedService(true);

                    // background workers
                    services.AddHostedService<MessagePublisher>();
                    services.AddHostedService<FifoMessagePublisher>();
                });
    }
}
