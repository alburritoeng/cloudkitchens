using kitchencli.api;
using kitchencli.utils;
using System;
using System.Collections.Generic;

namespace kitchencli.CourierMatch
{
    /// <summary>
    /// Manages the order handoff to a Courier once it is finished
    /// This implementations finds an the first available courier and sets its OrderId to the next avaiable order
    /// when more than 1 order is available, an arbitrary order is assigned to the Courier
    /// </summary>
    internal class CourierOrderMatch : ICourierOrderMatcher
    {
        private readonly ICourierPool _courierPool;
        internal readonly IDictionary<Guid, Tuple<Order, ICourier>> _matchingSet;
        private readonly object _lock = new object();
        private ICourierOrderTelemetry _telemetry;
        public CourierOrderMatch(ICourierPool courierPool, ICourierOrderTelemetry courierOrderTelemetry)
        {
            _courierPool = courierPool;
            _telemetry = courierOrderTelemetry;
            _matchingSet = new Dictionary<Guid, Tuple<Order, ICourier>>();
        }

        private bool Match(ICourier courier)
        {
            lock (_lock)
            {
                if (_matchingSet.ContainsKey(courier.OrderId()))
                {
                    PickupOrder(_matchingSet[courier.OrderId()].Item1, courier);
                    _matchingSet.Remove(courier.OrderId());
                    return true;
                }

                _matchingSet.Add(courier.OrderId(), new Tuple<Order, ICourier>(null, courier));
                return false;
            }
        }

        private bool Match(Order order)
        {
            lock (_lock)
            {
                Guid orderGuid;
                if (!Guid.TryParse(order.id, out orderGuid))
                {
                    return false;
                }
                if (_matchingSet.ContainsKey(orderGuid))
                {
                    PickupOrder(order,_matchingSet[orderGuid].Item2);
                    _matchingSet.Remove(orderGuid);
                    return true;
                }

                _matchingSet.Add(orderGuid, new Tuple<Order, ICourier>(order, null));
                return false;

            }
        }

        private void PickupOrder(Order order, ICourier courier)
        {
            // we have an entry for this OrderId, perhaps the order reached this point before courier
            // match them, deem the order delivered, and send the courier back to the Courier Factory
            Console.WriteLine($"{DateTime.Now.TimeOfDay} [CourierOrderMatch] Order {courier.OrderId()} picked up/delivered by Courier!");

            DateTime now = DateTime.Now;
            ((IOrderTelemetry) order).OrderPickUpTime = now;
            _telemetry.CalculateAverageFoodWaitTime(order);

            ((ICourierTelemetry) courier).OrderPickupTime = now;
            _telemetry.CalculateAverageCourierWaitTime(courier);
            
            _courierPool.ReturnCourier(courier);
            
            // notify of delivery
            OrderDeliveredPublisher.PublishOrderDelivered(order.id);
        }
        
        public void AddToOrderReadyQueue(Order order)
        {
            if (order == null)
            {
                return;
            }
            
            // clean up our delegate
            order.OrderReadyNotification -= this.AddToOrderReadyQueue;
            
            Console.WriteLine($"{DateTime.Now.TimeOfDay} [CourierOrderMatch] received ready order {order.id}-{order.name}");
            
            ((IOrderTelemetry)order).OrderReadyTime = DateTime.Now;
            
            var res = Match(order);
            if (res == false)
            {
                Console.WriteLine($"{DateTime.Now.TimeOfDay} [CourierOrderMach] Courier has not arrived for order {order.id}");
            }
        }

        public void CourierArrived(ICourier courier)
        {
            if (courier == null)
            {
                return;
            }

            if (Guid.TryParse(courier.CurrentOrder.id, out Guid guid))
            {
                if (guid == Guid.Empty)
                {
                    Console.WriteLine($"{DateTime.Now.TimeOfDay} [CourierOrderMatch] ERROR Courier {courier.CourierUniqueId} should have a valid order!");
                    return;
                }
            }
            
            Console.WriteLine($"{DateTime.Now.TimeOfDay} [CourierOrderMatch] Courier {courier.CourierUniqueId} has arrived for order");
            courier.ArrivalTime = DateTime.Now;
            
            var res = Match(courier);
            if (res == false)
            {
                Console.WriteLine($"{DateTime.Now.TimeOfDay} [CourierOrderMatch] Order {courier.OrderId()} not ready for Courier {courier.CourierUniqueId}");
            }
        }
        
        public MatchType GetMatchType()
        {
            return MatchType.Match;
        }
    }
}
