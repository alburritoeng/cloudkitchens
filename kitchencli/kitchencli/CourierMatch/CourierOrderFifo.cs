using kitchencli.api;
using kitchencli.utils;
using System;
using System.Collections.Generic;

namespace kitchencli.CourierMatch
{
    /// <summary>
    /// Manages the order handoff to a Courier once it is finished
    /// This implementations finds an exact Order Id to match against a courier's Order Id
    /// </summary>
    internal class CourierOrderFifo : ICourierOrderMatcher
    {
        private readonly object _lock = new object();
        private readonly object _courierLock = new object();
        private readonly List<Order> _readyForPickupOrders;
        private readonly Queue<ICourier> _idleCouriers;
        private ICourierFactory _courierFactory;
        private ICourierOrderTelemetry _telemetry;
        private Random random;
        public CourierOrderFifo(ICourierFactory courierFactory, ICourierOrderTelemetry courierOrderTelemetry)
        {
            _courierFactory = courierFactory;
            _telemetry = courierOrderTelemetry;
            _readyForPickupOrders = new List<Order>();
            _idleCouriers = new Queue<ICourier>();
            int seed = DateTime.Now.Millisecond;
            random = new Random(seed);
        }
        
        public void AddToOrderReadyQueue(Order order)
        {
            ((IOrderTelemetry)order).OrderReadyTime = DateTime.Now;
            lock (_lock)
            {
                lock (_courierLock)
                {
                    if (_idleCouriers.Count > 0)
                    {
                        // don't bother queuing this order, send it out to a courier who has been sitting
                        ICourier courier = _idleCouriers.Dequeue();
                        Console.WriteLine($"{DateTime.Now.TimeOfDay} [CourierOrderFifo] Found courier waiting in kitchen: {courier.CourierUniqueId}, handing ready order {order.id}");
                        HandOrderToCourier(order, courier);
                    }
                    else
                    {
                        _readyForPickupOrders.Add(order);    
                    }
                }
            }
        }

        private Order GetArbitraryOrder()
        {
            // just going to randomize a number with range of index 0 to List count-1
            lock (_lock)
            {
                if (_readyForPickupOrders.Count == 0)
                {
                    return null;
                }
                
                int arbitrary = random.Next(_readyForPickupOrders.Count);
                Order order = _readyForPickupOrders[arbitrary];
                _readyForPickupOrders.RemoveAt(arbitrary);
                return order;
            }
        }

        private void HandOrderToCourier(Order order, ICourier courier)
        {
            courier.CurrentOrder = order;
            DateTime now = DateTime.Now;
            ((IOrderTelemetry) order).OrderPickUpTime = now;
            _telemetry.CalculateAverageFoodWaitTime(order);

            ((ICourierTelemetry) courier).OrderPickupTime = now;
            _telemetry.CalculateAverageCourierWaitTime(courier);
            
            Console.WriteLine($"{DateTime.Now.TimeOfDay} [CourierOrderFifo] Courier {courier.CourierUniqueId} matched with order {courier.CurrentOrder.id}");
            Console.WriteLine($"{DateTime.Now.TimeOfDay} [CourierOrderFifo] Order {courier.OrderId()} picked up/delivered by Courier!");
            _courierFactory.ReturnCourier(courier);
        }
        
        public void CourierArrived(ICourier courier)
        {
            courier.ArrivalTime = DateTime.Now;
            Order order =  GetArbitraryOrder();
            if (order == null)
            {
                //add this courier to a wait queue for the next available order
                lock (_courierLock)
                {
                    Console.WriteLine($"{DateTime.Now.TimeOfDay} [CourierOrderFifo] No orders available for courier {courier.CourierUniqueId}, wait in kitchen"); 
                    _idleCouriers.Enqueue(courier);
                }

                return;
            }
            HandOrderToCourier(order, courier);
        }

        public MatchType GetMatchType()
        {
            return MatchType.Fifo;
        }
    }
}
