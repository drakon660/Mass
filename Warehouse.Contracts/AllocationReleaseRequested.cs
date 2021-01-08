using System;

namespace WareHouse.Contracts
{
    public interface AllocationReleaseRequested
    {
        Guid AllocationId { get; }
        string Reason { get; }
    }
}