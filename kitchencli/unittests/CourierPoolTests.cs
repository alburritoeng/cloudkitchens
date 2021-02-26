using System.Collections.Generic;
using kitchencli;
using kitchencli.api;
using NUnit.Framework;

namespace unittests.Properties
{
    [TestFixture]
    public class CourierPoolTests
    {
        [Test]
        public void GetCourier_PoolCount_Test()
        {
            CourierPool courierPool = new CourierPool();
            courierPool.Start();
            Assert.AreEqual(courierPool.MaxCouriers, courierPool._courierPoolSet.Count);
        }
        
        [Test]
        public void GetCourier_CanCreateExtraAsNeeded_Test()
        {
            CourierPool courierPool = new CourierPool();
            courierPool.Start();
            Assert.AreEqual(courierPool.MaxCouriers, courierPool._courierPoolSet.Count);
            List<ICourier> retrievedCouriers = new List<ICourier>();
            for (int i = 0; i < courierPool.MaxCouriers + 1; i++)
            {
                ICourier courier = courierPool.GetCourier();
                Assert.NotNull(courier);
                retrievedCouriers.Add(courier);
            }
            Assert.IsTrue(retrievedCouriers.Count > courierPool.MaxCouriers);
        }
        
        [Test]
        public void GetCourier_ReturnCouriersToPool_Test()
        {
            CourierPool courierPool = new CourierPool();
            courierPool.Start();
            Assert.AreEqual(courierPool.MaxCouriers, courierPool._courierPoolSet.Count);
            List<ICourier> retrievedCouriers = new List<ICourier>();
            for (int i = 0; i < courierPool.MaxCouriers; i++)
            {
                ICourier courier = courierPool.GetCourier();
                Assert.NotNull(courier);
                retrievedCouriers.Add(courier);
            }
            Assert.IsTrue(courierPool._courierPoolSet.Count ==0);

            foreach(ICourier courier in retrievedCouriers)
            {
                courierPool.ReturnCourier(courier);
            }
            
            Assert.IsTrue(courierPool._courierPoolSet.Count == retrievedCouriers.Count);
        }

        [Test]
        public void Stop_Test()
        {
            IStartStoppableModule courierPool = new CourierPool();
            courierPool.Start();
            
            Assert.DoesNotThrow(()=>
            {
                courierPool.Stop();
            });
        }
    }
}