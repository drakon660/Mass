using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Courier;
using WareHouse.Contracts;

namespace mass.components.CourierActivities
{
    public class AllocateInventoryActivity : IActivity<AllocateInventoryArguments,AllocateInventoryLog>
    {
        private readonly IRequestClient<AllocateInventory> _client;

        public AllocateInventoryActivity(IRequestClient<AllocateInventory> client)
        {
            _client = client;
        }
        
        public async Task<ExecutionResult> Execute(ExecuteContext<AllocateInventoryArguments> context)
        {
            var oderId = context.Arguments.OrderId;
            
            string itemNumber = context.Arguments.ItemNumber ?? 
                                throw new ArgumentNullException(nameof(itemNumber));
            
            var quantity = context.Arguments.Quantity;

            if (quantity <= 0.0m)
                throw new ArgumentNullException(nameof(quantity));

            var allocationId = NewId.NextGuid();

            Console.WriteLine("here");
            
            var response = await _client.GetResponse<InventoryAllocated>(new
            {
                AllocationId = allocationId,
                ItemNumber = itemNumber,
                Quantity = quantity
            });

            return context.Completed(new
            {
                AllocationId = allocationId,
            });
        }

        public async Task<CompensationResult> Compensate(CompensateContext<AllocateInventoryLog> context)
        {
            await context.Publish<AllocationReleaseRequested>(new
            {
                AllocationId = context.Log.AllocationId,
                Reason = "Order Faulted"
            });

            return context.Compensated();
        }
    }
}