using kitchencli.api;
using kitchencli.utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace kitchencli
{
    class CreatedOrderConsumer : IStartStoppableModule, IDisposable
    {
        private Queue<Order> _ordersQueue;
        private readonly object _lock = new object();
        private CancellationTokenSource cts;
        private IOrderReceiver _orderReceiver;
        private bool _disposed;
        private const int OneThousandMs = 1000;
        public CreatedOrderConsumer(IOrderReceiver orderReceiver)
        {
            _orderReceiver = orderReceiver;
            cts = new CancellationTokenSource();
            _ordersQueue = new Queue<Order>();
        }
        
        public void AddOrderToQueue(Order order) 
        {
            // using lock vs ConcurrentQueue because processing time of of picking an order is
            // very small, so we do not anticipate a slow performance here from locking to add and locking to remove
            lock(_lock)
            {
                _ordersQueue.Enqueue(order);
            }
        }

        /// <summary>
        /// Sending a null order means we do not have any items in the queue
        /// Caller of this method should properly handle receiving a null order
        /// </summary>
        /// <returns></returns>
        private Order PickOrder()
        {
            lock(_lock)
            {
                if(_ordersQueue.Count > 0 && _ordersQueue.Peek()!=null)
                {
                    return _ordersQueue.Dequeue();
                }
            }
            return null;
        }

        public void Stop()
        {
            cts.Cancel();
        }

        public void Start()
        {
            Task.Run(() =>
            {
                ProcessOrders(cts.Token);
            });
        }
        
        private async void ProcessOrders(CancellationToken token)
        {
            do
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine($"{DateTime.Now.TimeOfDay} [CreatedOrderConsumer] Order taking process has been stopped");
                    break;
                }

                // pick 2 orders/second
                SendSingleOrder();                
                try
                {
                    await Task.Delay(2 * OneThousandMs, token);
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

        private void SendSingleOrder()
        {
            // properly handle the use case of receiving a null order from the picker
            // we do not want to dispatch couriers or generate invalid orders
            Order order = PickOrder();
            if (order != null)
            {
                _orderReceiver.SendOrderToOrderMaker(order);
                _orderReceiver.DispatchCourier(order);
            }
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
