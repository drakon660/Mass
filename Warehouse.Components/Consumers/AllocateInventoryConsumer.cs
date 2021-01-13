using System;
using System.Threading.Tasks;
using MassTransit;

using WareHouse.Contracts;

namespace Warehouse.Components
{
    public class AllocateInventoryConsumer : IConsumer<AllocateInventory>
    {
        public async Task Consume(ConsumeContext<AllocateInventory> context)
        {
            await context.Publish<AllocationCreated>(new
            {
                context.Message.AllocationId,
                HoldDuration = 8000
            });

            await context.RespondAsync<InventoryAllocated>(values: new
            {
                AllocationId = context.Message.AllocationId,
                ItemNumber = context.Message.ItemNumber,
                Quantity = context.Message.Quantity
            });
        }
    }
}