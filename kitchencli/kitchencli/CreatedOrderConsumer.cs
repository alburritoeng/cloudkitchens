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
        public CreatedOrderConsumer(IOrderReceiver orderReceiver)
        {
            _orderReceiver = orderReceiver;
            cts = new CancellationTokenSource();
            _ordersQueue = new Queue<Order>();
        }
        
        public void AddOrderToQueue(Order order) 
        {
            // could use thread safe queue (out-of-the-box), but 
            // going to use a lock instead :)
            lock(_lock)
            {
                _ordersQueue.Enqueue(order);
            }
        }

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
                    await Task.Delay(1000, token);
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
            var order = PickOrder();
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
