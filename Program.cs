using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MassTransit;

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
                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<MessageConsumer>();

                        x.UsingAmazonSqs((context, cfg) =>
                        {
                            const string awsRegion = "ap-southeast-2";
                            cfg.Host(awsRegion, h =>
                            {
                                // Set the AWS Access key and Secrect here if its not configured in the environment
                                // h.AccessKey("your-iam-access-key");
                                // h.SecretKey("your-iam-secret-key");

                                // optional - specify a scope for all queues
                                h.Scope("dev");

                                // optional - scope topics as well
                                h.EnableScopedTopics();
                            });

                            cfg.ConfigureEndpoints(context);
                        });
                    });

                    services.AddMassTransitHostedService(true);

                    services.AddHostedService<MessagePublisher>();
                });
    }
}
