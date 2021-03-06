using System;
using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;

namespace mass.components.Consumers
{
    public class FulfillOrderConsumerDefinition : ConsumerDefinition<FullfillOrderConsumer>
    {
        public FulfillOrderConsumerDefinition()
        {
            ConcurrentMessageLimit = 4;
        }
        
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<FullfillOrderConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r =>
            {
                r.Ignore<InvalidOperationException>();
                r.Interval(3, 1000);
            });
            
            endpointConfigurator.DiscardFaultedMessages();
        }
    }

    
    //TODO consumer for faulted massages
}