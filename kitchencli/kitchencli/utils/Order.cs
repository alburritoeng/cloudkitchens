using System;
using System.Threading;
using System.Threading.Tasks;
using kitchencli.api;

namespace kitchencli.utils
{
    /// <summary>
    /// A concrete class to describe the JSON we will parse to create orders
    /// Input is in JSON, internally we will use C# objects to describe the JSON.
    /// Those C# objects will be the way we pass orders around internally through the workflow    
    /// </summary>
    public class Order: IOrderTelemetry
    {
        public Action<Order> OrderReadyNotification;
        
        public Order()
        {
            id = Guid.NewGuid().ToString();
            prepTime = -1;
            name = string.Empty;
        }
        /// <summary>
        /// The id of the Order
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// The name of the Order
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// The time for PrepTime (in seconds) for Order
        /// </summary>
        public int prepTime { get; set; }

        public void StartOrder()
        {
            Task.Run(() =>
            {
                using (ManualResetEvent evt = new ManualResetEvent(false))
                {
                    evt.WaitOne(prepTime * 1000);

                    if (OrderReadyNotification != null)
                    {
                        OrderReadyNotification(this);
                    }
                }
            });
        }

        public DateTime OrderReadyTime { get; set; }
        public DateTime OrderPickUpTime { get; set; }
    }   
    
}
