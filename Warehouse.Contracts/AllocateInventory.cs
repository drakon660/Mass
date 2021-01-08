﻿using System;

namespace WareHouse.Contracts
{
    public interface AllocateInventory
    {
        Guid AllocationId { get; }
        string ItemNumber { get; }
        decimal Quantity { get; }
    }
}