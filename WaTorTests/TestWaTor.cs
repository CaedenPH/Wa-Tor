using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
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
            Assert.AreEqual(entity.Coords, new Position (0, 0));
            Assert.IsTrue(entity.Alive);
            Assert.AreEqual(entity.RemainingReproductionTime, 5);
        }

        [TestMethod]
        public void EntityInitialization_Predator_CorrectValues()
        {
            var entity = new Entity(false, new Position(0, 0));

            Assert.IsFalse(entity.Prey);
            Assert.AreEqual(entity.Coords, new Position (0, 0));
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

    [TestClass]
    public class WaTorTests
    {
        [TestMethod]
        public void WaTorSetPlanet()
        {
            WaTor waTor = new WaTor(Constants.Width, Constants.Height);

            Entity[,] planet = { { null, null, null }, { new Entity(true, new Position(1, 0)), null, null } };
            waTor.SetPlanet(planet);

            Assert.AreEqual(waTor.Planet, planet);
            Assert.AreEqual(waTor.Width, planet.GetLength(0));
            Assert.AreEqual(waTor.Height, planet.GetLength(1));
        }

        [TestMethod]
        public void WaTorGetEntities_ReturnsEntities()
        {
            WaTor waTor = new WaTor(Constants.Width, Constants.Height);
            Assert.AreEqual(waTor.GetEntities().Where(entity => entity.Prey).Count(), Constants.PreyInitialCount);
            Assert.AreEqual(waTor.GetEntities().Where(entity => !entity.Prey).Count(), Constants.PredatorInitialCount);
        }

        [TestMethod]
        public void WaTorGetEntities_ReducesWhen_EntityDies()
        {
            WaTor waTor = new WaTor(Constants.Width, Constants.Height);
            
        }

        [TestMethod]
        public void WaTorAddEntity_AddsEntity()
        {
            WaTor waTor = new WaTor(Constants.Width, Constants.Height);
            waTor.AddEntity(true);

            waTor.AddEntity(false);
        }

        [TestMethod]
        public void WaTorBalancePredatorsAndPrey_BalancesEntities()
        {
            WaTor waTor = new WaTor(Constants.Width, Constants.Height);

            // Add lots of entities to be balanced
            for (int i = 0; i < 1000; i++)
            {
                int row = Math.DivRem(i, 2, out int col);
                waTor.Planet[row, col] = new Entity(true, new Position(row, col));
            }

            int numberOfEntities = waTor.GetEntities().Count();
            waTor.BalancePredatorsAndPrey();
            Assert.AreNotEqual(numberOfEntities, waTor.GetEntities().Count());
        }

        [TestMethod]
        public void WaTorGetSurroundPrey_GetsSurroundingPrey()
        {
            WaTor waTor = new WaTor(Constants.Width, Constants.Height);
            
        }

        [TestMethod]
        public void WaTorMoveAndReproduce()
        {
            WaTor waTor = new WaTor(Constants.Width, Constants.Height);
        }
    }
}