using System;
using mass.components.BatchConsumers;
using mass.components.Consumers;
using mass.components.CourierActivities;
using mass.components.StateMachines;
using mass.components.StateMachines.OrderStateMachineActivities;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.MongoDbIntegration;
using MassTransit.MongoDbIntegration.MessageData;
using MassTransit.Platform.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using WareHouse.Contracts;

namespace Mass.Startup
{
    public class MassStartup : IPlatformStartup
    {
        public void ConfigureMassTransit(IServiceCollectionConfigurator configurator, IServiceCollection services)
        {
            services.AddScoped<AcceptOrderActivity>();
            services.AddScoped<RoutingSlipBatchEventConsumer>();
            
            configurator.AddConsumersFromNamespaceContaining<SubmitOrderConsumer>();
            configurator.AddActivitiesFromNamespaceContaining<AllocateInventoryActivity>();

            configurator.AddConsumer<RoutingSlipBatchEventConsumer>(x=>x.Options<BatchOptions>(b=>b.SetMessageLimit(10).SetTimeLimit(s:1)));
            
            configurator.AddSagaStateMachine<OrderStateMachine, OrderState>(typeof(OrderStateMachineDefinition)).MongoDbRepository(cfg=> {
                cfg.Connection = "mongodb://mongo";
                cfg.DatabaseName = "orderdb"; 
            });
            
            configurator.AddRequestClient<AllocateInventory>();
        }

        public void ConfigureBus<TEndpointConfigurator>(IBusFactoryConfigurator<TEndpointConfigurator> configurator, IRegistrationContext<IServiceProvider> context) 
            where TEndpointConfigurator : IReceiveEndpointConfigurator
        {
            configurator.UseMessageData(new MongoDbMessageDataRepository("mongodb://127.0.0.1", "attachments"));
            //configurator.UseMessageScheduler(new Uri("queue:quartz"));
        }
    }
}