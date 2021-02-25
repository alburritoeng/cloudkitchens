using kitchencli.utils;

namespace kitchencli.Couriers
{
    internal class GrubHubCourier : Courier
    {        
        public GrubHubCourier(Order order)
        {
            CurrentOrder = order;
        }

        public override string CourierType()
        {
            return "GrubHub";
        }      
        
        public override int CourierTypeInt()
        {
            return 3;
        }
    }
}
