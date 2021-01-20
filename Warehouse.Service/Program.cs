using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Definition;
using MassTransit.MongoDbIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Warehouse.Components;
using Warehouse.Components.StateMachines;

namespace Warehouse.Service
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
                        cfg.AddConsumersFromNamespaceContaining<AllocateInventoryConsumer>();
                        cfg.AddSagaStateMachine<AllocationStateMachine, AllocationState>(typeof(AllocateStateMachineDefinition))
                            .MongoDbRepository(x=> {
                            x.Connection = "mongodb://127.0.0.1";
                            x.DatabaseName = "allocations";
                        });
                        
                        cfg.UsingRabbitMq((context, configurator) =>
                        {
                            configurator.UseMessageScheduler(new Uri("rabbitmq://localhost/quartz"));
                            configurator.ConfigureEndpoints(context);
                        });
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
    }
    
}