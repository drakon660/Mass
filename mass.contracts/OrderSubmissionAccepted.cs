﻿using System;

namespace mass.contracts
{
    public interface OrderSubmissionAccepted
    {
        Guid OrderId { get; }
        DateTime Timestamp { get; }

        string CustomerNumber { get; }
        string PaymentCardNumber { get; }
    }
}
