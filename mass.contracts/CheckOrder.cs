using System;
using System.Collections.Generic;
using System.Text;

namespace mass.contracts
{
    public interface CheckOrder
    {
        Guid OrderId { get; }
    }
}
