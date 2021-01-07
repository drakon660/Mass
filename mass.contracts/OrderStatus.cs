using System;

namespace mass.contracts
{
    public interface OrderStatus
    {
        Guid OrderId { get; }
        string CustomerNumber { get; }
        string State { get; }
    }
}
