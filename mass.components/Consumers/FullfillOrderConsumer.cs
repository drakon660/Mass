using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Courier;

namespace mass.components.Consumers
{
    public class FullfillOrderConsumer : IConsumer<FullfillOrder>
    {
        public async Task Consume(ConsumeContext<FullfillOrder> context)
        {
            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            builder.AddActivity("AllocateInventory", 
                new Uri("queue:allocate-inventory_execute"),new
            {
                ItemNumber = "ITEM123",
                Quantity = 10.0m
            });
            
            builder.AddVariable("orderId", context.Message.OrderId);
            var routingSlip = builder.Build();
            await context.Execute(routingSlip);
        }
    }
}