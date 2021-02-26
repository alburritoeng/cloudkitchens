using kitchencli.utils;

namespace kitchencli.Couriers
{
    internal class GrubHubCourier : Courier
    {
        public GrubHubCourier()
        {
            
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
