using kitchencli.api;
using kitchencli.utils;
using System;
using System.Collections.Generic;
using System.Security.Permissions;
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
    internal class KitchenModule : IKitchen, IStartStoppableModule, IDisposable
    {
        private readonly ICourierOrderMatcher _foodMatcher;
        internal CancellationTokenSource cts;
        private readonly object _lock = new object();
        private Queue<Order> _ordersQueue;
        private bool _disposed;
        public KitchenModule(ICourierOrderMatcher foodMatcher)
        {
            _foodMatcher = foodMatcher;
            _ordersQueue = new Queue<Order>();
            cts = new CancellationTokenSource();
        }

        public void PrepareOrder(Order order)
        {
            lock (_lock)
            {
                _ordersQueue.Enqueue(order);
            }
        }

        public void CourierHasArrived(ICourier courier)
        {
            Task.Run(() =>
            {
                Console.WriteLine($"{DateTime.Now.TimeOfDay} [KitchenModule] Courier {courier.CourierUniqueId} has arrived to pick up order");
                _foodMatcher.CourierArrived(courier);
                //because we are returning this courier to the courier pool after it is done, we must clear our delegate
                courier.NotifyArrivedForOrder -= this.CourierHasArrived;
            });
        }

        public void Start()
        {
            // I felt this is how many chefs was a realistic thing to have in a kitchen ?
            Task.Run(() =>
            {
                ChefsClockIn("Gordon Ramsay", cts.Token);
            });
            
            Task.Run(() =>
            {
                ChefsClockIn("Wolfgang Puck",cts.Token);
            });
            
            Task.Run(() =>
            {
                ChefsClockIn("Guy Fieri",cts.Token);
            });
            
            Task.Run(() =>
            {
                ChefsClockIn("Rachael Ray",cts.Token);
            });
            
            Task.Run(() =>
            {
                ChefsClockIn("Bobby Flay",cts.Token);
            });
            
            Task.Run(() =>
            {
                ChefsClockIn("Happy Gilmore",cts.Token);
            });
            
            Task.Run(() =>
            {
                ChefsClockIn("Billy Madison",cts.Token);
            });
            
        }

        private async void ChefsClockIn(string chefName, CancellationToken token)
        {
            do
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine($"{DateTime.Now.TimeOfDay} [KitchenModule] {chefName} is now on break!");
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
                    //PrepareOrder(order);
                    order.OrderReadyNotification += _foodMatcher.AddToOrderReadyQueue;
                    order.StartOrder();
                    Console.WriteLine($"{DateTime.Now.TimeOfDay} [KitchenModule] {chefName} is starting Order {order.id}-{order.name} ETA {order.prepTime}");
                }

                int waitTime = 100;
                if (order != null)
                {
                    waitTime = order.prepTime * 1000;
                }
                try
                {
                    await Task.Delay(waitTime, token); // chef-working on an order, should be ready to pick up another from the queue when prepTime is up
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    //task is cancelled, return or do something else
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
                
            } while (true);
        }
        
        public void Stop()
        {
            cts.Cancel();
        }

        public void Dispose()
        {
            Dispose(true);
        }
        
        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                cts?.Dispose();
            }

            _disposed = true;
        }
    }
}
