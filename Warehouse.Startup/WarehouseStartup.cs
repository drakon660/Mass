using System;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.MongoDbIntegration;
using MassTransit.Platform.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Components;
using Warehouse.Components.StateMachines;

namespace Warehouse.Startup
{
    public class WarehouseStartup : IPlatformStartup
    {
        public void ConfigureMassTransit(IServiceCollectionBusConfigurator configurator, IServiceCollection services)
        {
            configurator.AddConsumersFromNamespaceContaining<AllocateInventoryConsumer>();
            configurator.AddSagaStateMachine<AllocationStateMachine, AllocationState>(typeof(AllocateStateMachineDefinition))
                .MongoDbRepository(cfg=> {
                    cfg.Connection = "mongodb://mongo";
                    cfg.DatabaseName = "allocations";
                });;
        }

        public void ConfigureBus<TEndpointConfigurator>(IBusFactoryConfigurator<TEndpointConfigurator> configurator, IBusRegistrationContext context) 
            where TEndpointConfigurator : IReceiveEndpointConfigurator
        {
          
        }
    }
}