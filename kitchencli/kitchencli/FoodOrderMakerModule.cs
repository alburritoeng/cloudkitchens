using kitchencli.api;
using kitchencli.utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace kitchencli
{
    /// <summary>
    /// Handles receiving an Order from the IOrderReceiver
    /// The orders will added to an OrderTicket object that has a timer delegate
    /// When the timer expires, the delegate is called and passes the order to the
    /// IFoodOrderStatusManager, where it will be handed to a Courier
    /// </summary>
    internal class FoodOrderMakerModule : IFoodOrderMaker, IStartStoppableModule
    {
        private readonly ICourierOrderMatcher _foodMatcher;
        private CancellationTokenSource cts;
        private readonly object _lock = new object();
        private Queue<Order> _ordersQueue;
        public FoodOrderMakerModule(ICourierOrderMatcher foodMatcher)
        {
            _foodMatcher = foodMatcher;
            _ordersQueue = new Queue<Order>();
            cts = new CancellationTokenSource();
        }

        private void OrderUp(Order order)
        {
            lock (_lock)
            {
                _ordersQueue.Enqueue(order);
            }
        }

        public void PrepareOrder(Order order)
        {
            order.OrderReadyNotification += _foodMatcher.AddToOrderReadyQueue;
            order.StartOrder();
            Console.WriteLine($"{DateTime.Now.TimeOfDay} [FoodOrderMakerModule] Starting Order {order.id}-{order.name} ETA {order.prepTime}");
        }

        public void CourierHasArrived(ICourier courier)
        {
            
            Task.Run(() =>
            {
                Console.WriteLine($"{DateTime.Now.TimeOfDay} [FoodOrderMakerModule] Courier {courier.CourierUniqueId} has arrived to pick up order");
                _foodMatcher.CourierArrived(courier);
            });
        }

        public void Start()
        {
            Task.Run(() =>
            {
                ChefsClockIn(cts.Token);
            });
            
            Task.Run(() =>
            {
                ChefsClockIn(cts.Token);
            });
            
            Task.Run(() =>
            {
                ChefsClockIn(cts.Token);
            });
            
        }

        private async void ChefsClockIn(CancellationToken token)
        {
            do
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine($"Chefs on break!");
                    break;
                }

                Order order = null;
                lock (_lock)
                {
                    if (_ordersQueue.Count > 0)
                    {
                        order = _ordersQueue.Dequeue();
                    }
                }

                if (order != null)
                {
                    PrepareOrder(order);
                }

                await Task.Delay(100, token); // chef-break
                
            } while (true);
        }
        
        public void Stop()
        {
            cts.Cancel();
        }
    }
}
