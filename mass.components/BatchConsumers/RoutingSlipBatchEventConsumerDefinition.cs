using MassTransit.Definition;

namespace mass.components.BatchConsumers
{
    public class RoutingSlipBatchEventConsumerDefinition : ConsumerDefinition<RoutingSlipBatchEventConsumer>
    {
        public RoutingSlipBatchEventConsumerDefinition()
        {
            ConcurrentMessageLimit = 20;
            
        }
    }
}