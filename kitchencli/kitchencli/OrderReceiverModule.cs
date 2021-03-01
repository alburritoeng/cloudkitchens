using kitchencli.api;
using kitchencli.utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace kitchencli
{
    /// <summary>
    /// The purpose of the class is to receive an order from the Cli
    /// The order is in the JSON format specified
    /// An order is created from the JSON 
    /// A courier is dispatched for the order
    /// An Order object is dispatched to the IFoodOrderStatusManager object
    /// </summary>
    internal class OrderReceiverModule : IOrderReceiver, IStartStoppableModule, IDisposable
    {
        IKitchen _kitchen;
        ICourierPool _courierPool;
        private ICourierOrderMatcher _courierOrderMatcher;
        internal Queue<Order> _receivedOrdersQueue;
        private Queue<Order> _courierQueue;
        private readonly object _ordersLock = new object();
        private readonly object _courierLock = new object();
        internal CancellationTokenSource cts;
        private const int WaitBetweenQueueReadsMs = 100;
        private bool _disposed;
        public OrderReceiverModule(IKitchen kitchen, ICourierPool courierPool, ICourierOrderMatcher courierOrderMatcher)
        {
            _kitchen = kitchen;
            _courierPool = courierPool;
            _courierOrderMatcher = courierOrderMatcher;
            _receivedOrdersQueue = new Queue<Order>();
            _courierQueue = new Queue<Order>();
            cts = new CancellationTokenSource();
        }

        public void DispatchCourier(Order order)
        {
            lock (_courierLock)
            {
                _courierQueue.Enqueue(order);
            }
        }

        public void SendOrderToOrderMaker(Order order)
        {
            lock (_ordersLock)
            {
                _receivedOrdersQueue.Enqueue(order);
            }
        }

        private async void OrderWorker(CancellationToken token)
        {
            do
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine($"Stopping Order Receiver");
                    break;
                }

                Order order = null;
                lock (_ordersLock)
                {
                    if (_receivedOrdersQueue.Count > 0)
                    {
                        order = _receivedOrdersQueue.Dequeue();
                    }
                }

                if (order != null)
                {
                    Console.WriteLine($"{DateTime.Now.TimeOfDay} [OrderReceiverModule] Sending order {order.id}-{order.name} to OrderMaker, ETA {order.prepTimeSeconds}");
                    _kitchen.PrepareOrder(order);
                }

                try
                {
                    await Task.Delay(WaitBetweenQueueReadsMs, token);
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

        private async void DispatchCourierWorker(CancellationToken token)
        {
            do
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine($"Stopping Courier Dispatcher Worker");
                    break;
                }

                Order order = null;
                lock (_courierLock)
                {
                    if (_courierQueue.Count > 0)
                    {
                        order = _courierQueue.Dequeue();
                    }
                }

                if (order != null)
                {
                    ICourier courier = _courierPool.GetCourier();

                    if (courier != null)
                    {
                        //ask the ICourierOrderMatcher for an order (if this is match, otherwise, we won't know what order to pick up)
                        if (_courierOrderMatcher.GetMatchType() == MatchType.Match)
                        {
                            courier.CurrentOrder = order;
                        }
                        else
                        {
                            courier.CurrentOrder = null; // we will find you an order when you arrive at pick up 
                        }

                        Console.WriteLine(
                            $"{DateTime.Now.TimeOfDay} [OrderReceiverModule] Dispatching Courier {courier.CourierUniqueId} on the way for order {courier.CurrentOrder?.id}, ETA {courier.DurationEstimateInSeconds()}");
                        courier.NotifyArrivedForOrder += _kitchen.CourierHasArrived;
                        courier.LeaveForFood();
                    }
                }

                try
                {
                    await Task.Delay(WaitBetweenQueueReadsMs, token);
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

        public void Start()
        {
            Task.Run(() =>
            {
                OrderWorker(cts.Token);
            });
            
            Task.Run(() =>
            {
                DispatchCourierWorker(cts.Token);
            });
            
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
