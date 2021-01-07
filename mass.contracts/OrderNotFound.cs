using System;

namespace mass.contracts
{
    public interface OrderNotFound
    {
        Guid OrderId { get; }
    }
}