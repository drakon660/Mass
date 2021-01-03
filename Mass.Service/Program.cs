using mass.components.Consumers;
using MassTransit;
using MassTransit.Definition;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Mass.Service
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
               .ConfigureAppConfiguration((hostingContext, config) =>
               {
                   config.AddJsonFile("appsettings.json", true);
                   config.AddEnvironmentVariables();

                   if (args != null)
                       config.AddCommandLine(args);
               })
               .ConfigureServices((hostContext, services) =>
               {
                   services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);

                   services.AddMassTransit(cfg =>
                   {
                       cfg.AddConsumersFromNamespaceContaining<SubmitOrderConsumer>();
                       //cfg.UsingRabbitMq(ConfigureBus);
                       cfg.AddBus(ConfigureBus);
                       //cfg.AddRequestClient<AllocateInventory>();
                   });

                   services.AddHostedService<MassTransitConsoleHostedService>();
               })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    //logging.AddSerilog(dispose: true);
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                });


            await builder.RunConsoleAsync();
        }
        static IBusControl ConfigureBus(IServiceProvider provider)
        {
            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.ConfigureEndpoints(provider);

            });
        }
    }
}
