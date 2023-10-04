using System;
using System.Collections.Generic;

namespace WaTorSimulation
{
    public class Entity
    {
        public bool Prey { get; set; }
        public (int, int) Coords { get; set; }
        public bool Alive { get; set; } = true;
        public int RemainingReproductionTime { get; set; }
        public int? EnergyValue { get; set; }

        public Entity(bool prey, (int, int) coords)
        {
            Prey = prey;
            Coords = coords;
            RemainingReproductionTime = prey ? Constants.PreyReproductionTime : Constants.PredatorReproductionTime;
            EnergyValue = prey ? null : (int?)Constants.PredatorInitialEnergyValue;
        }

        public void ResetReproductionTime()
        {
            RemainingReproductionTime = Prey ? Constants.PreyReproductionTime : Constants.PredatorReproductionTime;
        }

        public override string ToString()
        {
            string repr = $"Entity(Prey={Prey}, Coords={Coords}, RemainingReproductionTime={RemainingReproductionTime}";
            if (EnergyValue.HasValue)
            {
                repr += $", EnergyValue={EnergyValue}";
            }
            return repr + ")";
        }
    }

    public static class Constants
    {
        public const int Width = 50;
        public const int Height = 50;
        public const int PreyInitialCount = 30;
        public const int PreyReproductionTime = 5;
        public const int PredatorInitialCount = 50;
        public const int PredatorInitialEnergyValue = 15;
        public const int PredatorFoodValue = 5;
        public const int PredatorReproductionTime = 20;
        public const int MaxEntities = 500;
        public const int DeleteUnbalancedEntities = 50;
    }

    public class WaTor
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public Entity[,] Planet { get; set; }

        public WaTor(int width, int height)
        {
            Width = width;
            Height = height;
            Planet = new Entity[Height, Width];

            // Initialize with initial prey and predator entities
            // [Your logic here]
        }

        public void AddEntity(bool prey)
        {
            // Logic for adding entity
        }

        public List<Entity> GetEntities()
        {
            // Logic for getting entities
            return new List<Entity>();
        }

        public void BalancePredatorsAndPrey()
        {
            // Logic for balancing
        }

        public List<Entity> GetSurroundingPrey(Entity entity)
        {
            // Logic for getting surrounding prey
            return new List<Entity>();
        }

        public void MoveAndReproduce(Entity entity, List<string> directionOrders)
        {
            // Logic for moving and reproducing
        }

        public void PerformPreyActions(Entity entity, List<string> directionOrders)
        {
            // Logic for performing prey actions
        }

        public void PerformPredatorActions(Entity entity, (int, int)? occupiedByPreyCoords, List<string> directionOrders)
        {
            // Logic for performing predator actions
        }

        public void Run(int iterationCount)
        {
            // Logic for running simulation
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            WaTor waTor = new WaTor(Constants.Width, Constants.Height);
            waTor.Run(100_000);
        }
    }
}
