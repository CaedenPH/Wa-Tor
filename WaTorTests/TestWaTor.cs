using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
            Assert.AreEqual(entity.Coords, new Position(0, 0));
            Assert.IsTrue(entity.Alive);
            Assert.AreEqual(entity.RemainingReproductionTime, 5);
        }

        [TestMethod]
        public void EntityInitialization_Predator_CorrectValues()
        {
            var entity = new Entity(false, new Position(0, 0));

            Assert.IsFalse(entity.Prey);
            Assert.AreEqual(entity.Coords, new Position(0, 0));
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
        public void WaTorSetPlanet_TestOne()
        {
            WaTor waTor = new WaTor(Constants.Width, Constants.Height);

            Entity[,] planet = { { null, null, null }, { new Entity(true, new Position(1, 0)), null, null } };
            waTor.SetPlanet(planet);

            Assert.AreEqual(waTor.Planet, planet);
            Assert.AreEqual(3, waTor.Width);
            Assert.AreEqual(2, waTor.Height);
        }

        [TestMethod]
        public void WaTorSetPlanet_TestTwo()
        {
            WaTor waTor = new WaTor(Constants.Width, Constants.Height);
            Entity[,] planet = { { new Entity(false, new Position(0, 0)) }, { null }, { null }, { null } };
            waTor.SetPlanet(planet);

            Assert.AreEqual(1, waTor.Width);
            Assert.AreEqual(4, waTor.Height);
        }

        [TestMethod]
        public void WaTorGetEntities_ReturnsEntities()
        {
            WaTor waTor = new WaTor(Constants.Width, Constants.Height);
            Assert.AreEqual(waTor.GetEntities().Where(entity => entity.Prey).Count(), Constants.PreyInitialCount);
            Assert.AreEqual(waTor.GetEntities().Where(entity => !entity.Prey).Count(), Constants.PredatorInitialCount);
        }

        [TestMethod]
        public void WaTorAddEntity_AddsEntity()
        {
            WaTor waTor = new WaTor(Constants.Width, Constants.Height);
            waTor.AddEntity(true);
            waTor.AddEntity(false);
            waTor.AddEntity(false);

            Assert.AreEqual(Constants.PreyInitialCount + 1, waTor.GetEntities().Where(entity => entity.Prey).Count());
            Assert.AreEqual(Constants.PredatorInitialCount + 2, waTor.GetEntities().Where(entity => entity.Prey != true).Count());

        }

        [TestMethod]
        public void WaTorBalancePredatorsAndPrey_BalancesEntities()
        {
            WaTor waTor = new WaTor(Constants.Width, Constants.Height);

            // Add lots of entities to be balanced
            for (int i = 0; i < 1000; i++)
            {
                int row = Math.DivRem(i, Constants.Height, out int _);
                int col = i % Constants.Width;
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
            Entity predator = new Entity(false, new Position(1, 1));

            Entity[,] planet =
            {
                { null, new Entity(true, new Position(0, 1)), null },
                { null, predator, null },
                { null, new Entity(true, new Position(2, 1)), null }
            };

            var surroundingPrey = waTor.GetSurroundingPrey(predator);
            var expectedSurroundingPrey = new List<Entity>() { new Entity(true, new Position(0, 1)),
                new Entity(true, new Position(2, 1)) };

            int i = 0;
            foreach (Entity prey in surroundingPrey)
            {
                Assert.AreEqual(prey, expectedSurroundingPrey[i]);
                i++;
            }
        }

        [TestMethod]
        public void WaTorMoveAndReproduce_DoesReproduce()
        {
            WaTor waTor = new WaTor(Constants.Width, Constants.Height);

            Entity reproduceableEntity = new Entity(false, new Position(0, 0));
            reproduceableEntity.RemainingReproductionTime = 0;
            Assert.AreEqual(0, reproduceableEntity.RemainingReproductionTime);

            Entity[,] planet = { { reproduceableEntity, null } };

            waTor.SetPlanet(planet);
            waTor.MoveAndReproduce(reproduceableEntity, new List<string>() { "E" });

            Assert.AreEqual(false, waTor.Planet[0, 0].Prey);
            Assert.AreEqual(0, waTor.Planet[0, 0].Coords.Row);
            Assert.AreEqual(0, waTor.Planet[0, 0].Coords.Col);
            Assert.AreEqual(reproduceableEntity, waTor.Planet[0, 1]);
        }

        [TestMethod]
        public void WaTorPerformPredatorActions_EatsPrey()
        {
            WaTor waTor = new WaTor(Constants.Height, Constants.Width);
            Entity predator = new Entity(true, new Position(0, 0));

            Entity[,] planet =
            {
                { predator, new Entity(false, new Position(0, 1))}
            };
            waTor.SetPlanet(planet);
            waTor.PerformPredatorActions(predator, new List<string>(), new Position(0, 1));

            Assert.AreEqual(waTor.Planet[0, 0], null);
        }

        [TestMethod]
        public void WaTorRun_Entities_Change()
        {
            WaTor waTor = new WaTor(Constants.Height, Constants.Width);
            waTor.Run(Constants.PredatorInitialEnergyValue - 1, display: false);

            int predatorCount = waTor.GetEntities().Where(entity => entity.Prey != true).Count();
            Assert.IsTrue(predatorCount >= Constants.PredatorInitialCount);
        }
    }
}