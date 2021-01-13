using System;

namespace mass.contracts
{
    public interface OrderFulFillmentCompleted
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }
    }
}