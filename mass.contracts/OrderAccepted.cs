using System;

namespace mass.contracts
{
    public interface OrderAccepted    
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }
    }
}
