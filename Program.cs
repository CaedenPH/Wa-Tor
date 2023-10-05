using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;

namespace WaTorSimulation
{
    public readonly struct Position
    {
        public int Row { get; }
        public int Col { get; }

        public Position(int row, int col)
        {
            Row = row;
            Col = col;
        }
    }

    public class Entity
    {
        public bool Prey { get; set; }
        public Position Coords { get; set; }
        public bool Alive { get; set; } = true;
        public int RemainingReproductionTime { get; set; }
        public int? EnergyValue { get; set; }

        public Entity(bool prey, Position coords)
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

    /// <summary>
    /// Wa-Tor algorithm (1984)
    /// 
    /// This solution aims to completely remove any systematic approach
    /// to the Wa-Tor planet, and utilise fully random methods.
    /// 
    /// The constants are a working set that allows the Wa-Tor planet
    /// to result in one of the three possible results.
    /// 
    /// References:
    /// https://en.wikipedia.org/wiki/Wa-Tor
    /// https://beltoforion.de/en/wator/
    /// https://beltoforion.de/en/wator/images/wator_medium.webm
    /// </summary>
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

            // Populate planet with predators and prey randomly
            for (int i = 0; i > Constants.PreyInitialCount; i++)
            {
                this.AddEntity(true);
            }
            for (int i = 0; i > Constants.PreyInitialCount; i++)
            {
                this.AddEntity(false);
            }
        }

        /// <summary>
        /// Adds an entity, making sure the entity does not override another entity.
        /// </summary>
        /// <remarks>This function can iterate infinitely if there is not enough space on the Wa-Tor planet.</remarks>
        /// <param name="prey">Whether the entity is prey or not</param>
        public void AddEntity(bool prey)
        {
            while (true)
            {
                int row = new Random().Next(0, this.Height - 1);
                int column = new Random().Next(0, this.Width - 1);
                Position position = new Position(row, column);

                if (this.Planet[row, column] != null)
                {
                    this.Planet[row, column] = new Entity(prey, position);
                    return;
                }
            }
        }

        /// <summary>
        /// Returns a list of all the entities within the planet.
        /// </summary>
        /// <remarks></remarks>
        /// <returns></returns>
        public List<Entity> GetEntities()
        {
            var entities = new List<Entity>();

            for (int i = 0; i < Planet.GetLength(0); i++)
            {
                for (int j = 0; j < Planet.GetLength(1); j++)
                {
                    if (Planet[i, j] != null)
                    {
                        entities.Add(Planet[i, j]);
                    }
                }
            }
            return entities;
        }

        /// <summary>
        /// Balances predators and preys so that prey can not dominate the predators
        /// by blocking up space for them to reproduce.
        /// </summary>
        public void BalancePredatorsAndPrey()
        {
            var entities = this.GetEntities();

            if (entities.Count >= Constants.MaxEntities - Constants.MaxEntities / 10)
            {
                var prey = entities.Where(entity => entity.Prey is true).ToList();
                var predators = entities.Where(entity => entity.Prey is false).ToList();

                int preyCount = prey.Count;
                int predatorCount = predators.Count;

                var entitiesToPurge = new List<Entity>();
                if (preyCount > predatorCount)
                {
                    entitiesToPurge.AddRange(prey.Take(Constants.DeleteUnbalancedEntities));
                } else
                {
                    entitiesToPurge.AddRange(predators.Take(Constants.DeleteUnbalancedEntities));
                }

                // Purge each entity
                foreach (Entity entity in entitiesToPurge)
                {
                    this.Planet[entity.Coords.Row, entity.Coords.Col] = null;
                }
            }
        }

        /// <summary>
        /// Returns all the prey entities around (N, S, E, W) a predator entity.
        /// </summary>
        /// <remarks>Subtly different to the <c>MoveAndReproduce</c> square.</remarks>
        /// <param name="entity">The entity to get the surrounding prey for</param>
        public List<Entity> GetSurroundingPrey(Entity entity)
        {
            int row = entity.Coords.Row;
            int col = entity.Coords.Col;

            int[,] adjacent = {
                { row - 1, col },  // North
                { row + 1, col},  // South
                { row, col - 1},  // West
                { row, col + 1},  // East
            };

            var entities = new List<Entity>();
            for (int i = 0; i < adjacent.GetLength(0); i ++)
            {
                int r = adjacent[i, 0];
                int c = adjacent[i, 1];

                if (0 <= r && r < this.Height && 0 <= c && c < this.Width)
                {
                    Entity ent = this.Planet[r, c];
                    if (ent != null && ent.Prey) entities.Add(ent);
                }
            }
            return entities;
        }

        /// <summary>
        /// Attempts to move to an unoccupied neighbouring square in either of the four directions(North, South, East, West). 
        /// If the move was successful and the remaining_reproduction time is equal to 0
        /// then a new prey or predator can also be created in the previous square.
        /// </summary>
        /// <param name="entity">The entity to move and reproduce</param>
        /// <param name="directionOrders">Ordered list (like priority queue) depicting
        /// order to attempt to move. Removes any systematic approach of checking neighbouring squares.</param>
        public void MoveAndReproduce(Entity entity, List<string> directionOrders)
        {
            int row = entity.Coords.Row;
            int col = entity.Coords.Col;

            var adjacentSquares = new Dictionary<string, (int, int)>
            {
                { "N", (row - 1, col) },  // North
                { "S", (row + 1, col) },  // South
                { "W", (row, col - 1) },  // West
                { "E", (row, col + 1) },  // East
            };

            var adjacent = new List<(int, int)>();
            foreach (var order in directionOrders)
            {
                adjacent.Add(adjacentSquares[order]);
            }

            foreach (var (r, c) in adjacent)
            {
                if (0 <= r && r < this.Height && 0 <= c && c < this.Width && this.Planet[r, c] == null)
                {
                    // Move entity to empty adjacent square
                    this.Planet[r, c] = entity;
                    this.Planet[row, col] = null;
                    entity.Coords = new Position(r, c);
                    break;
                }
            }

            // Check if it is possible to reproduce in the previous square
            if (entity.Coords.Row != row && entity.RemainingReproductionTime <= 0)
            {
                // Check if the entities on the planet are less than the max limit
                if (GetEntities().Count < Constants.MaxEntities)
                {
                    // Reproduce in the previous square
                    this.Planet[row, col] = new Entity(entity.Prey, new Position(row, col));
                    entity.ResetReproductionTime();
                }
            }
            else
            {
                entity.RemainingReproductionTime -= 1;
            }
        }

        /// <summary>
        /// Performs the actions for a prey entity

        /// For prey the rules are:
        ///   1. At each chronon, a prey moves randomly to one of the adjacent unoccupied
        ///    squares.If there are no free squares, no movement takes place.
        ///   2. Once a prey has survived a certain number of chronons it may reproduce.
        ///    This is done as it moves to a neighbouring square,
        ///    leaving behind a new prey in its old surroundingPreyCoords.
        ///    Its reproduction time is also reset to zero.
        /// </summary>
        /// <param name="entity">The prey entity to perform actions for</param>
        /// <param name="directionOrders">Ordered list (like priority queue) depicting
        /// order to attempt to move. Removes any systematic approach of checking neighbouring squares.</param>
        public void PerformPreyActions(Entity entity, List<string> directionOrders)
        {
            this.MoveAndReproduce(entity, directionOrders);
        }

        /// <summary>
        /// Performs the actions for a predator entity
        /// 
        /// For predators the rules are:
        /// 1. At each chronon, a predator moves randomly to an adjacent square occupied
        ///  by a prey.If there is none, the predator moves to a random adjacent
        ///  unoccupied square.If there are no free squares, no movement takes place.
        /// 2. At each chronon, each predator is deprived of a unit of energy.
        /// 3. Upon reaching zero energy, a predator dies.
        /// 4. If a predator moves to a square occupied by a prey,
        ///  it eats the prey and earns a certain amount of energy.
        /// 5. Once a predator has survived a certain number of chronons
        ///  it may reproduce in exactly the same way as the prey.
        /// </summary>
        /// <param name="entity">The prey entity to move</param>
        /// <param name="occupiedByPreyCoords">List of positions that are occupied by prey for the predator to eat.</param>
        /// <param name="directionOrders">Ordered list (like priority queue) depicting
        /// order to attempt to move. Removes any systematic approach of checking neighbouring squares.</param>
        public void PerformPredatorActions(Entity entity, Position? occupiedByPreyCoords, List<string> directionOrders)
        {
            int row = entity.Coords.Row;
            int col = entity.Coords.Col;

            // (3.) If the entity has 0 energy, it will die
            if (entity.EnergyValue == 0)
            {
                this.Planet[row, col] = null;
                return;
            }

            // (1.) Move to entity if possible
            if (occupiedByPreyCoords != null)
            {
                // (5.) If it has survived the certain number of chronons it will also
                // reproduce in this function
                this.MoveAndReproduce(entity, directionOrders);
            }
            else
            {
                // Kill the prey
                Entity prey = this.Planet[occupiedByPreyCoords.Row, occupiedByPreyCoords.Col];
                prey.Alive = false;

                // Move onto prey
                this.Planet[occupiedByPreyCoords.Row, occupiedByPreyCoords.Col] = entity;
                this.Planet[row, col] = null;

                    entity.Coords = occupiedByPreyCoords;
                    // (4.) Eats the prey and gains energy
                    entity.EnergyValue += Constants.PredatorFoodValue;
            }

            entity.EnergyValue -= 1;
        }

        public void DisplayPlanet() {

        }

        /// <summary>
        /// Emulate time passing by looping iteration_count times
        /// </summary>
        /// <param name="iterationCount">Number of cronons (time intervals)</param>
        public void Run(int iterationCount)
        {
            for (int cronon = 0;  cronon < iterationCount; cronon++)
            {
                // Generate list of all entities in order to randomly
                // pop an entity at a time to simulate true randomness
                // This removes the systematic approach of iterating
                // through each entity width by height
                var entities = this.GetEntities();

                // Perform either prey/predator actions on every entity in the planet
                for (int _ = 0; _ <  entities.Count; _++)
                {
                    // Randomly pick an entity and ensure it is alive
                    var entity = entities[_];
                    entities.RemoveAt(_);

                    if (entity.Alive is false) continue;

                    var directions = new List<string>(){ "N", "E", "S", "W" };

                    if (entity.Prey is true)
                    {
                        this.PerformPreyActions(entity, directions);
                    } else
                    {
                        var surroundingPrey = this.GetSurroundingPrey(entity);
                        Position? surroundingPreyCoords = null;

                        if (surroundingPrey != null)
                        {
                            surroundingPreyCoords = surroundingPrey[0].Coords;   
                        }

                        this.PerformPredatorActions(entity, surroundingPreyCoords, directions);
                    }
                }

                this.BalancePredatorsAndPrey();
                this.DisplayPlanet();
            }
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
