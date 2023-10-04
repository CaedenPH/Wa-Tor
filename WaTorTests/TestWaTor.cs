using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using WaTorSimulation;


namespace WaTorTests
{
    [TestClass]
    public class EntityTests
    {
        [TestMethod]
        public void EntityInitialization_Prey_CorrectValues()
        {
            var entity = new Entity(true, new Position(0, 0));

            Assert.IsTrue(entity.Prey);
            Assert.AreEqual(entity.Coords, (0, 0));
            Assert.IsTrue(entity.Alive);
            Assert.AreEqual(entity.RemainingReproductionTime, 5);
        }

        [TestMethod]
        public void EntityInitialization_Predator_CorrectValues()
        {
            var entity = new Entity(false, new Position(0, 0));

            Assert.IsFalse(entity.Prey);
            Assert.AreEqual(entity.Coords, (0, 0));
            Assert.IsTrue(entity.Alive);
            Assert.AreEqual(entity.RemainingReproductionTime, 20);
            Assert.AreEqual(entity.EnergyValue, 15);
        }

        [TestMethod]
        public void EntityResetReproductionTime_Prey_ResetsTo5()
        {
            var entity = new Entity(true, new Position(0, 0));
            entity.RemainingReproductionTime = 0;

            entity.ResetReproductionTime();

            Assert.AreEqual(entity.RemainingReproductionTime, 5);
        }

        [TestMethod]
        public void EntityResetReproductionTime_Predator_ResetsTo20()
        {
            var entity = new Entity(false, new Position(0, 0));
            entity.RemainingReproductionTime = 0;

            entity.ResetReproductionTime();

            Assert.AreEqual(entity.RemainingReproductionTime, 20);
        }
    }
}