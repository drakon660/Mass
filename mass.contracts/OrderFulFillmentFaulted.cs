using System;
using MassTransit.Courier.Contracts;

namespace mass.contracts
{
    public interface OrderFulFillmentFaulted
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }
    }
}