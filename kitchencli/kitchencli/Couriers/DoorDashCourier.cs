﻿using kitchencli.api;
using kitchencli.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kitchencli.Couriers
{
    class DoorDashCourier : Courier
    {       
        public DoorDashCourier(Order order)
        {
            _currentOrder = order;
        }

        public override string CourierType()
        {
            return "DoorDash";
        }       
    }
}
