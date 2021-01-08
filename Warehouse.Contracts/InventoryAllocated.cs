using System;

namespace WareHouse.Contracts
{
    public interface InventoryAllocated
    {
        Guid AllocationId { get; }
        string ItemNumber { get; }
        decimal Quantity { get; }
    }
}