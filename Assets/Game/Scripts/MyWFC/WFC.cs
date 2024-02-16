using RH;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace Assets.Game.Scripts.MyWFC
{
    public class WFC : MonoBehaviour
    {
        public int Width = 5;
        public int Height = 5;
        public ConfigTile ConfigTile;

        public ClassTile[,] Tiles;

        public Stack<ClassTile> stackTiles;

        public Tilemap tilemap;
        public Tile[] tiles;

        private RH.TileDataModel _tileDataModel;

        public void Start()
        {
            _tileDataModel = ConfigTile.Convert();
            Tiles = new ClassTile[Width, Height];

            List<ClassTileType> AllTileTypes = new List<ClassTileType>();

            foreach (KeyValuePair<string, int> kv in _tileDataModel.TileTypes)
            {
                ClassTileType classTileType = new ClassTileType(kv.Key, kv.Value);
                classTileType.Weight = _tileDataModel.TileWeights[kv.Key];
                AllTileTypes.Add(classTileType);
            }

            foreach (KeyValuePair<string, List<string>> kv in _tileDataModel.TileRules)
            {
                ClassTileType tileType = AllTileTypes.Find(x => x.Name == kv.Key);
                for (int i = 0; i < kv.Value.Count; i++)
                {
                    ClassTileType t = AllTileTypes.Find(x => x.Name == kv.Value[i]);
                    tileType.AddRule((ClassTileEdge) i, t);
                }
            }

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Tiles[x, y] = new ClassTile(x, y, AllTileTypes);
                }
            }

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (x > 0)
                    {
                        Tiles[x, y].AddNeighbour(Tiles[x - 1, y]);
                    }
                    if (x < Width - 1)
                    {
                        Tiles[x, y].AddNeighbour(Tiles[x + 1, y]);
                    }
                    if (y > 0)
                    {
                        Tiles[x, y].AddNeighbour(Tiles[x, y - 1]);
                    }
                    if (y < Height - 1)
                    {
                        Tiles[x, y].AddNeighbour(Tiles[x, y + 1]);
                    }
                }
            }

            Run();
        }

        public ClassTile t1;
        public ClassTile t2;

        public void RunTest()
        {
            Tiles[0, 0].PossibleTiles = new List<ClassTileType> { Tiles[0, 0].PossibleTiles[1] };
            // Collapse(0, 0);
            // Propagate the changes
            //bool state = Propagate(0, 0);
            ClassTileType tileToConstrain = null;
            // Top neighbour
            foreach (ClassTileType neighbourPossibleTile in new List<ClassTileType>(Tiles[0, 1].PossibleTiles))
            {
                bool constrain = true;
                // Current tile possibilities
                foreach (ClassTileType possibleTile in Tiles[0, 0].PossibleTiles)
                {
                    // Bottom rule contains possible tile?
                    if (neighbourPossibleTile.Rules[(int)ClassTileEdge.Top].Id == possibleTile.Id)
                    {
                        constrain = false;
                    }
                }
                if (constrain)
                {
                    Tiles[0, 1].PossibleTiles.Remove(neighbourPossibleTile);
                }
            }

            t1 = Tiles[1, 0];
            t2 = Tiles[0, 1];
            Draw(0, 0);
        }

        public void Draw(int x, int y)
        {
            tilemap.SetTile(new Vector3Int(x, y, 0), tiles[Tiles[x, y].PossibleTiles[0].Id]);
        }

        public void DrawAll()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (Tiles[x, y].PossibleTiles.Count <= 0)
                    {
                        Debug.LogError("No possible tiles");
                    }
                    else
                    { 
                        tilemap.SetTile(new Vector3Int(x, y, 0), tiles[Tiles[x, y].PossibleTiles[0].Id]);
                    }
                }
            }
        }

        public void Run()
        {
            // Collaspe this tile
            Collapse(0, 0);
            // Propagate the changes
            bool state = Propagate(0, 0);



            int iterationCount = 0;
            bool error = false;
            while(!IsFullyCollapsed() && !error)
            {
                Debug.Log($"Iteration count {iterationCount}");
                if (iterationCount > 1000 || error)
                {
                    Debug.LogError("Too many iterations");
                    break;
                }
                error = Iterate();
                if (error) 
                    Debug.Log($"Error at iteraction {iterationCount}");
                iterationCount++;
            }
            Debug.Log("All tiles collapsed");
            DrawAll();
        }

        bool IsFullyCollapsed()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (Tiles[x, y].PossibleTiles.Count > 1)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool Iterate()
        {
            // Get tile with lowest entropy
            (int x, int y) coordsLowestEntropy = GetLowestEntropyTile();
            Debug.Log($"Lowest entropy tile at {coordsLowestEntropy.x}, {coordsLowestEntropy.y}");
            // Collaspe this tile
            Collapse(coordsLowestEntropy.x, coordsLowestEntropy.y);
            // Propagate the changes
            bool state = Propagate(coordsLowestEntropy.x, coordsLowestEntropy.y);
            return state;
        }

        public List<ClassTile> stackContent;


        public void Propagate2(int x, int y)
        {
            stackTiles = new Stack<ClassTile>();

            stackTiles.Push(Tiles[x, y]);
            int iterationCount = 0;

            while (stackTiles.Count > 0)
            {
                if (iterationCount > 1000)
                {
                    Debug.LogError("Too many iterations");
                    Debug.LogError($"stackTiles.Count {stackTiles.Count}");
                    return;
                }

                ClassTile tile = stackTiles.Pop();
                // Possible tiles for the current tile
                List<ClassTileType> possibleTiles = tile.PossibleTiles;

                foreach (ClassTile neighbour in tile.Neighbours)
                {
                    if (neighbour.PossibleTiles.Count > 1)
                    {

                    }
                }
            }
        }

        public void GetDirections(ClassTile tile)
        {

        }

        public bool Propagate(int x, int y)
        {
            stackTiles = new Stack<ClassTile>();

            stackTiles.Push(Tiles[x, y]);
            int iterationCount = 0;

            while (stackTiles.Count > 0)
            {
                stackContent = stackTiles.ToList();
                if (iterationCount > 1000)
                {
                    Debug.LogError("Too many iterations");
                    Debug.LogError($"stackTiles.Count {stackTiles.Count}");
                    return true;
                }

                Debug.Log($"Stack count {stackTiles.Count}");
                ClassTile tile = stackTiles.Pop();
                // Possible tiles for the current tile
                List<ClassTileType> possibleTiles = tile.PossibleTiles;

                bool constrain = true;
                bool reduced = false;
                ClassTileType tileToConstrain = null;
                if (tile.Y < Height - 1)
                {
                    ClassTile tileUp = Tiles[tile.X, tile.Y + 1];
                    if (tileUp.PossibleTiles.Count > 1)
                    {
                        foreach (ClassTileType neighbourPossibleTile in new List<ClassTileType>(tileUp.PossibleTiles))
                        {
                            constrain = true;
                            foreach (ClassTileType possibleTile in possibleTiles)
                            {
                                if (neighbourPossibleTile.Rules[(int)ClassTileEdge.Bottom].Id == possibleTile.Id)
                                {
                                    constrain = false;
                                }
                            }
                            if (constrain)
                            {
                                reduced = true;
                                tileUp.PossibleTiles.Remove(neighbourPossibleTile);
                            }
                        }
                    }
                    if (reduced)
                    {
                        stackTiles.Push(tileUp);
                    }
                }

                reduced = false;
                if (tile.X < Width - 1)
                {
                    ClassTile tileRight = Tiles[tile.X + 1, tile.Y];
                    if (tileRight.PossibleTiles.Count > 1)
                    {
                        foreach (ClassTileType neighbourPossibleTile in new List<ClassTileType>(tileRight.PossibleTiles))
                        {
                            constrain = true;
                            foreach (ClassTileType possibleTile in possibleTiles)
                            {
                                if (neighbourPossibleTile.Rules[(int)ClassTileEdge.Left].Id == possibleTile.Id)
                                {
                                    constrain = false;
                                }
                            }
                            if (constrain)
                            {
                                reduced = true;
                                tileRight.PossibleTiles.Remove(neighbourPossibleTile);
                            }
                        }
                    }

                    if (reduced)
                    {
                        stackTiles.Push(tileRight);
                    }
                }

                reduced = false;
                if (tile.Y > 0)
                {
                    ClassTile tileDown = Tiles[tile.X, tile.Y - 1];
                    if (tileDown.PossibleTiles.Count > 1)
                    {
                        foreach (ClassTileType neighbourPossibleTile in new List<ClassTileType>(tileDown.PossibleTiles))
                        {
                            constrain = true;
                            foreach (ClassTileType possibleTile in possibleTiles)
                            {
                                if (neighbourPossibleTile.Rules[(int)ClassTileEdge.Top].Id == possibleTile.Id)
                                {
                                    constrain = false;
                                }
                            }
                            if (constrain)
                            {
                                reduced = true;
                                tileDown.PossibleTiles.Remove(neighbourPossibleTile);
                            }
                        }
                    }

                    if (reduced)
                    {
                        stackTiles.Push(tileDown);
                    }
                        
                }

                reduced = false;
                if (tile.X > 0)
                {
                    ClassTile tileLeft = Tiles[tile.X - 1, tile.Y];

                    if (tileLeft.PossibleTiles.Count > 1)
                    {
                        foreach (ClassTileType neighbourPossibleTile in new List<ClassTileType>(tileLeft.PossibleTiles))
                        {
                            constrain = true;
                            foreach (ClassTileType possibleTile in possibleTiles)
                            {
                                if (neighbourPossibleTile.Rules[(int)ClassTileEdge.Right].Id == possibleTile.Id)
                                {
                                    constrain = false;
                                }
                            }
                            if (constrain)
                            {
                                reduced = true;
                                tileLeft.PossibleTiles.Remove(neighbourPossibleTile);
                            }
                        }
                    }
                    
                    if (reduced)
                    {
                        stackTiles.Push(tileLeft);
                    }
                }
                iterationCount++;
            }
            return false;
        }

        public void Collapse(int x, int y)
        {
            ClassTile tile = Tiles[x, y];
            List<int> weights = new List<int>();
            for (int i = 0; i < tile.PossibleTiles.Count; i++)
            {
                weights.Add(tile.PossibleTiles[i].Weight);
            }

            int rdmChoice = WeightedRandomSelect(weights.ToArray());
            if (x == 0 && y == 0)
            {
                rdmChoice = 5;
            }
            tile.PossibleTiles = new List<ClassTileType> { tile.PossibleTiles[rdmChoice] };
        }

        public (int, int) GetLowestEntropyTile()
        {
            float minEntropy = float.MaxValue;
            (int, int) chosenCoords = (-1, -1);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    ClassTile tile = Tiles[x, y];

                    if (tile.PossibleTiles.Count == 1)
                    {
                        continue;
                    }

                    float entropy = ShannonEntropy(tile.PossibleTiles);
                    float noise = UnityEngine.Random.Range(0f, 1f) / 1000f;
                    if (entropy - noise < minEntropy)
                    {
                        minEntropy = entropy - noise;
                        chosenCoords = (x, y);
                    }
                }
            }
            return chosenCoords;
        }

        float ShannonEntropy(List<ClassTileType> possibleTiles)
        {
            float sum_of_weights = 0;
            float sum_of_weight_log_weights = 0;

            foreach (ClassTileType tile in possibleTiles)
            {
                float weight = tile.Weight;
                sum_of_weights += weight;
                sum_of_weight_log_weights += weight * Mathf.Log(weight);
            }
            return Mathf.Log(sum_of_weights) - (sum_of_weight_log_weights / sum_of_weights);
        }

        int WeightedRandomSelect(int[] weights)
        {
            float totalWeight = 0f;
            for (int i = 0; i < weights.Length; i++)
            {
                totalWeight += weights[i];
            }

            float randomPoint = UnityEngine.Random.value * totalWeight;

            for (int i = 0; i < weights.Length; i++)
            {
                if (randomPoint < weights[i])
                {
                    return i;
                }
                randomPoint -= weights[i];
            }

            return -1; // Should not happen
        }
    }

    [Serializable]
    public class ClassTile
    {
        public int X;
        public int Y;
        public List<ClassTile> Neighbours;
        public List<ClassTileType> PossibleTiles;

        public ClassTile(int x, int y, List<ClassTileType> possibleTiles)
        {
            X = x;
            Y = y;
            PossibleTiles = new List<ClassTileType>(possibleTiles);
            Neighbours = new List<ClassTile>();
        }

        public void AddNeighbour(ClassTile neighbour)
        {
            Neighbours.Add(neighbour);
        }
    }

    public enum ClassTileEdge
    {
        Top = 0,
        Right = 1,
        Bottom = 2,
        Left = 3
    }

    [Serializable]
    public class ClassTileType
    {
        public string Name;
        public int Id;
        public int Weight;
        [NonSerialized]
        public ClassTileType[] Rules;


        public ClassTileType(string name, int id, int weight = 1)
        {
            Name = name;
            Id = id;
            Weight = weight;
            Rules = new ClassTileType[4];
        }

        public void AddRule(ClassTileEdge dir, ClassTileType rule)
        {
            Rules[(int)dir] = rule;
        }
    }
}
