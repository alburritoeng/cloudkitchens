using kitchencli.utils;

namespace kitchencli.Couriers
{
    internal class DoorDashCourier : Courier
    {       
        public DoorDashCourier(Order order)
        {
            CurrentOrder = order;
        }

        public override string CourierType()
        {
            return "DoorDash";
        }

        public override int CourierTypeInt()
        {
            return 2;
        }
    }
}
