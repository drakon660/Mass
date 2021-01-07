using GreenPipes;
using MassTransit;
using MassTransit.Definition;

namespace mass.components.StateMachines
{
    public class OrderStateMachineDefinition : SagaDefinition<OrderState>
    {
        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderState> sagaConfigurator)
        {
            //TOOD partitioning what is it?


            endpointConfigurator.UseMessageRetry(r => r.Intervals(500,5000,10000));
            endpointConfigurator.UseInMemoryOutbox();
        }
    }

}
