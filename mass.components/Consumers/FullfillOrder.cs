using System;

namespace mass.components.Consumers
{
    public interface FullfillOrder
    {
        Guid OrderId { get; }
    }
}