using mass.components.Consumers;
using mass.components.StateMachines;
using MassTransit;
using MassTransit.Definition;
using MassTransit.MongoDbIntegration;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using mass.components.BatchConsumers;
using mass.components.CourierActivities;
using mass.components.StateMachines.OrderStateMachineActivities;
using MassTransit.Courier.Contracts;
using WareHouse.Contracts;

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
                   //TODO check applicationinsight telemetry
                   //services.AddApplicationInsightsTelemetry();
                   
                   services.AddScoped<RoutingSlipBatchEventConsumer>();
                   services.AddScoped<AcceptOrderActivity>();
                   services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);

                   services.AddMassTransit(cfg =>
                   {
                       cfg.AddConsumersFromNamespaceContaining<SubmitOrderConsumer>();
                       cfg.AddConsumersFromNamespaceContaining<FullfillOrderConsumer>();
                       cfg.AddConsumersFromNamespaceContaining<RoutingSlipEventConsumer>();
                       cfg.AddActivitiesFromNamespaceContaining<AllocateInventoryActivity>();
                       
                       cfg.AddSagaStateMachine<OrderStateMachine, OrderState>(typeof(OrderStateMachineDefinition)).MongoDbRepository(cfg=> {
                           cfg.Connection = "mongodb://127.0.0.1";
                           cfg.DatabaseName = "orderdb";
                       });
                       //cfg.UsingRabbitMq(ConfigureBus);
                       cfg.AddBus(ConfigureBus);
                       cfg.AddRequestClient<AllocateInventory>();
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

        private static IBusControl ConfigureBus(IRegistrationContext<IServiceProvider> arg)
        {
            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {   
                cfg.UseMessageScheduler(new Uri("queue:quartz"));
                cfg.ConfigureEndpoints(arg);
                cfg.ReceiveEndpoint(KebabCaseEndpointNameFormatter.Instance.Consumer<RoutingSlipBatchEventConsumer>(),
                    e =>
                    {
                        e.PrefetchCount = 20;
                        e.Batch<RoutingSlipCompleted>(b=>
                        {
                            b.MessageLimit = 10;
                            b.TimeLimit = TimeSpan.FromSeconds(5);
                            b.Consumer<RoutingSlipBatchEventConsumer, RoutingSlipCompleted>(arg.Container);
                        });
                    });
                    

            });
        }

        //obsolete
        //static IBusControl ConfigureBus(IServiceProvider provider)
        //{
        //    return Bus.Factory.CreateUsingRabbitMq(cfg =>
        //    {
        //        cfg.ConfigureEndpoints(provider);

        //    });
        //}
    }
}
