using System;

namespace kitchencli.utils
{
    public class OrderDeliveredPublisher
    {
        private static Action<string> _orderDelivered;

        public static void Subscribe(Action<string> subscriber)
        {
            _orderDelivered += subscriber;
        }

        public static void PublishOrderDelivered(string orderId)
        {
            _orderDelivered?.Invoke(orderId);
        }
    }
}