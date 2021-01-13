using System;

namespace WareHouse.Contracts
{
    public interface AllocationHoldDurationExpired
    {
        Guid AllocationId { get; }
    }
}