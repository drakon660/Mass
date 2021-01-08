using System;

namespace mass.contracts
{
    public interface CustomerAccountClosed
    {
        Guid CustomerId { get; set; }
        string CustomerNumber { get; set; }        
    }
}
