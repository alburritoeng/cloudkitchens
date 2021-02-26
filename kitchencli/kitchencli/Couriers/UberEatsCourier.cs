using kitchencli.utils;

namespace kitchencli.Couriers
{
    internal class UberEatsCourier : Courier
    {
        public UberEatsCourier()
        {
            
        }

        public override string CourierType()
        {
            return "UberEats";
        }

        public override int CourierTypeInt()
        {
            return 1;
        }
    }
}
