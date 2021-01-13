using System;

namespace mass.components.CourierActivities
{
    public interface PaymentArguments
    {
        Guid OrderId { get; }
        decimal Amount { get; }
        string CardNumber { get; }
    }
}