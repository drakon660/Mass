using System;

namespace mass.components.Consumers
{
    public interface FullfillOrder
    {
        Guid OrderId { get; }
        string CustomerNumber { get; }
        string PaymentCardNumber { get; }
    }
}