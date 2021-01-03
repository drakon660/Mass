using MassTransit;
using System;

namespace mass.contracts
{
    public interface SubmitOrder
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }
        string CustomerNumber { get; }

        string PaymentCardNumber { get; }

        MessageData<string> Notes { get; }
    }

    public interface OrderSubmitted
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }

        string CustomerNumber { get; }
        string PaymentCardNumber { get; }

        MessageData<string> Notes { get; }
    }
}
