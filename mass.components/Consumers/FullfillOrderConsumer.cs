using System;
using System.Threading.Tasks;
using mass.contracts;
using MassTransit;
using MassTransit.Courier;
using MassTransit.Courier.Contracts;

namespace mass.components.Consumers
{
    public class FullfillOrderConsumer : IConsumer<FullfillOrder>
    {
        public async Task Consume(ConsumeContext<FullfillOrder> context)
        {
            if (context.Message.CustomerNumber.StartsWith("INVALID"))
            {
                throw new InvalidOperationException("We tried, but customer is invalid");
            }

            if (context.Message.CustomerNumber.StartsWith("MAYBE"))
            {
                throw new ApplicationException("We randomly exploded, so bad, much tear.");
            }

            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            builder.AddActivity("AllocateInventory",
                new Uri("queue:allocate-inventory_execute"), new
                {
                    ItemNumber = "ITEM123",
                    Quantity = 10.0m
                });

            builder.AddActivity("PaymentActivity",
                new Uri("queue:payment_execute"), new
                {
                    CardNumber = context.Message.PaymentCardNumber ?? "5999-1234-1234-1234",
                    Amount = 99.95m
                });

            await builder.AddSubscription(context.SourceAddress,
                RoutingSlipEvents.Faulted | RoutingSlipEvents.Supplemental,
                RoutingSlipEventContents.None, x => x.Send<OrderFulFillmentFaulted>(new {context.Message.OrderId}));

            await builder.AddSubscription(context.SourceAddress,
                RoutingSlipEvents.Completed | RoutingSlipEvents.Supplemental,
                RoutingSlipEventContents.None, x => x.Send<OrderFulFillmentCompleted>(new {context.Message.OrderId}));

            builder.AddVariable("orderId", context.Message.OrderId);
            var routingSlip = builder.Build();
            await context.Execute(routingSlip);
        }
    }
}