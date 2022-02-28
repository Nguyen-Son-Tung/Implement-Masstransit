using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sample.Components.Consumers;
using System.Threading.Tasks;

namespace Sample.Service
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }
        private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddJsonFile("appsettings.json");
                builder.AddEnvironmentVariables();
            })
            .ConfigureServices((context, service) =>
            {
                service.AddMassTransit(cfg =>
                {
                    cfg.UsingRabbitMq((context, rbCfg) =>
                    {
                        rbCfg.Host("rabbitmq://localhost", h =>
                        {
                            h.Username("tung");
                            h.Password("123456");
                        });

                        rbCfg.ConfigureEndpoints(context);
                    });

                    cfg.AddConsumersFromNamespaceContaining<SubmitOrderConsumer>();

                    cfg.SetKebabCaseEndpointNameFormatter();


                });

                service.AddMassTransitHostedService();

            })
            .ConfigureLogging((context, logging) =>
            {
                logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                logging.AddConsole();
            });
    }
}
