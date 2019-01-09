﻿using StockManagementSystem.Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockManagementSystem.Core.Domain.Settings
{
    public class OrderLimitStore : Entity
    {
        public int OrderLimitId { get; set; }

        public int StoreId { get; set; }

        public virtual OrderLimit OrderLimit { get; set; }

        public virtual Store Store { get; set; }
    }
}