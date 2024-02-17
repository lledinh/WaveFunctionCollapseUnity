using RH;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.MyWFC
{
    public class WFC
    {
        public int Width = 5;
        public int Height = 5;

        public ClassTile[,] Tiles;
        public Stack<ClassTile> stackTiles;
        [NonSerialized] public List<ClassTile> TilesList;
        [NonSerialized] public List<ClassTile> TileCollapseOrder;
        private const int TOP = 0;
        private const int RIGHT = 1;
        private const int BOTTOM = 2;
        private const int LEFT = 3;

        RH.TileDataModel _tileDataModel;

        public WFC(int width, int height, RH.TileDataModel tileDataModel)
        {
            Width = width;
            Height = height;
            _tileDataModel = tileDataModel;
        }

        public void Init(int seed)
        {
            TileCollapseOrder = new List<ClassTile>();
            Tiles = new ClassTile[Width, Height];
            TilesList = new List<ClassTile>();

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
                    tileType.AddRule((ClassTileEdge)i, kv.Value[i]);
                }
            }

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Tiles[x, y] = new ClassTile(x, y, AllTileTypes);
                    TilesList.Add(Tiles[x, y]);
                }
            }

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (y < Height - 1)
                    {
                        Tiles[x, y].AddNeighbour(Tiles[x, y + 1]);
                    }
                    else
                    {
                        Tiles[x, y].AddNeighbour(null);
                    }

                    if (x < Width - 1)
                    {
                        Tiles[x, y].AddNeighbour(Tiles[x + 1, y]);
                    }
                    else
                    {
                        Tiles[x, y].AddNeighbour(null);
                    }

                    if (y > 0)
                    {
                        Tiles[x, y].AddNeighbour(Tiles[x, y - 1]);
                    }
                    else
                    {
                        Tiles[x, y].AddNeighbour(null);
                    }

                    if (x > 0)
                    {
                        Tiles[x, y].AddNeighbour(Tiles[x - 1, y]);
                    }
                    else
                    {
                        Tiles[x, y].AddNeighbour(null);
                    }
                }
            }
        }

        public bool Iterate(int x = -1, int y = -1)
        {
            if (x == -1 && y == -1)
            {
                (x, y) = GetLowestEntropyTile2();
                if (x == -1 && (y == -1)) { return true; }
            }
            /*else
            {
                List<ClassTileType> t = Tiles[x, y].PossibleTiles;
                Tiles[x, y].PossibleTiles = new List<ClassTileType>
                {
                    t[5]
                };
            }*/

            TileCollapseOrder.Add(Tiles[x, y]);
            Collapse(x, y);

            Stack<ClassTile> stackTiles = new Stack<ClassTile>();
            stackTiles.Push(Tiles[x, y]);
            // SEED 1111 Collapse 0,0 DIRT_W => 1,0 DIRT_E => 0,1 DIRT_NW
            while (stackTiles.Count > 0)
            {
                ClassTile tile = stackTiles.Pop();
                List<ClassTileType> possibleTiles = new List<ClassTileType>(tile.PossibleTiles);
                List<ClassTile> neighbours = tile.Neighbours;

                for (int i = 0; i < neighbours.Count; i++)
                {
                    ClassTile neighbour = neighbours[i];
                    if (neighbour == null)
                    {
                        continue;
                    }
                    // Nieghbour not collapsed
                    //if (neighbour.PossibleTiles.Count > 1)
                    if (!neighbour.Collapsed)
                    {
                        List<string> rulesForCurrentTile = new List<string>();
                        if (i == TOP)
                        {
                            // reduced = neighbour.constrain(tilePossibilities, direction)
                            for (int iz = 0; iz < possibleTiles.Count; iz++)
                            {
                                string ruleTop = possibleTiles[iz].Rules[TOP];
                                rulesForCurrentTile.Add(ruleTop);
                            }

                            List<ClassTileType> copyNeighbourPossibleTiles = new List<ClassTileType>(neighbour.PossibleTiles);
                            for (int iw = 0; iw < copyNeighbourPossibleTiles.Count; iw++)
                            {
                                // neighbour.PossibleTiles[iw].Rules[2]
                                bool isInRule = false;
                                for (int iy = 0; iy < rulesForCurrentTile.Count; iy++)
                                {
                                    if (copyNeighbourPossibleTiles[iw].Rules[BOTTOM] == rulesForCurrentTile[iy])
                                    {
                                        isInRule = true;
                                    }
                                }
                                if (!isInRule)
                                {
                                    neighbour.PossibleTiles.Remove(copyNeighbourPossibleTiles[iw]);
                                }
                            }
                        }

                        rulesForCurrentTile = new List<string>();
                        if (i == RIGHT)
                        {
                            // reduced = neighbour.constrain(tilePossibilities, direction)
                            for (int iz = 0; iz < possibleTiles.Count; iz++)
                            {
                                string ruleTop = possibleTiles[iz].Rules[RIGHT];
                                rulesForCurrentTile.Add(ruleTop);
                            }

                            List<ClassTileType> copyNeighbourPossibleTiles = new List<ClassTileType>(neighbour.PossibleTiles);
                            for (int iw = 0; iw < copyNeighbourPossibleTiles.Count; iw++)
                            {
                                // neighbour.PossibleTiles[iw].Rules[2]
                                bool isInRule = false;
                                for (int iy = 0; iy < rulesForCurrentTile.Count; iy++)
                                {
                                    if (copyNeighbourPossibleTiles[iw].Rules[LEFT] == rulesForCurrentTile[iy])
                                    {
                                        isInRule = true;
                                    }
                                }
                                if (!isInRule)
                                {
                                    neighbour.PossibleTiles.Remove(copyNeighbourPossibleTiles[iw]);
                                }
                            }
                        }

                        rulesForCurrentTile = new List<string>();
                        if (i == BOTTOM)
                        {
                            // reduced = neighbour.constrain(tilePossibilities, direction)
                            for (int iz = 0; iz < possibleTiles.Count; iz++)
                            {
                                string ruleTop = possibleTiles[iz].Rules[BOTTOM];
                                rulesForCurrentTile.Add(ruleTop);
                            }

                            List<ClassTileType> copyNeighbourPossibleTiles = new List<ClassTileType>(neighbour.PossibleTiles);
                            for (int iw = 0; iw < copyNeighbourPossibleTiles.Count; iw++)
                            {
                                // neighbour.PossibleTiles[iw].Rules[2]
                                bool isInRule = false;
                                for (int iy = 0; iy < rulesForCurrentTile.Count; iy++)
                                {
                                    if (copyNeighbourPossibleTiles[iw].Rules[TOP] == rulesForCurrentTile[iy])
                                    {
                                        isInRule = true;
                                    }
                                }
                                if (!isInRule)
                                {
                                    neighbour.PossibleTiles.Remove(copyNeighbourPossibleTiles[iw]);
                                }
                            }
                        }

                        rulesForCurrentTile = new List<string>();
                        if (i == LEFT)
                        {
                            // reduced = neighbour.constrain(tilePossibilities, direction)
                            for (int iz = 0; iz < possibleTiles.Count; iz++)
                            {
                                string ruleTop = possibleTiles[iz].Rules[LEFT];
                                rulesForCurrentTile.Add(ruleTop);
                            }

                            List<ClassTileType> copyNeighbourPossibleTiles = new List<ClassTileType>(neighbour.PossibleTiles);
                            for (int iw = 0; iw < copyNeighbourPossibleTiles.Count; iw++)
                            {
                                // neighbour.PossibleTiles[iw].Rules[2]
                                bool isInRule = false;
                                for (int iy = 0; iy < rulesForCurrentTile.Count; iy++)
                                {
                                    if (copyNeighbourPossibleTiles[iw].Rules[RIGHT] == rulesForCurrentTile[iy])
                                    {
                                        isInRule = true;
                                    }
                                }
                                if (!isInRule)
                                {
                                    neighbour.PossibleTiles.Remove(copyNeighbourPossibleTiles[iw]);
                                }
                            }
                        }
                    }
                }
            }

            return false;
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

        public void Collapse(int x, int y)
        {
            ClassTile tile = Tiles[x, y];
            List<int> weights = new List<int>();
            for (int i = 0; i < tile.PossibleTiles.Count; i++)
            {
                weights.Add(tile.PossibleTiles[i].Weight);
            }

            int rdmChoice = WeightedRandomSelect(weights.ToArray());
            tile.PossibleTiles = new List<ClassTileType> { tile.PossibleTiles[rdmChoice] };
            tile.Collapsed = true;
        }

        public (int, int) GetLowestEntropyTile2()
        {
            float minEntropy = float.MaxValue;
            (int, int) chosenCoords = (-1, -1);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    ClassTile tile = Tiles[x, y];

                    if (tile.Collapsed)
                    {
                        continue;
                    }

                    float entropy = tile.PossibleTiles.Count;
                    //float noise = UnityEngine.Random.Range(0f, 1f) / 1000f;
                    //if (entropy - noise < minEntropy)
                    if (entropy < minEntropy)
                    {
                        //minEntropy = entropy - noise;
                        minEntropy = entropy;
                        chosenCoords = (x, y);
                    }
                }
            }
            return chosenCoords;
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

                    if (tile.Collapsed)
                    {
                        continue;
                    }

                    float entropy = ShannonEntropy(tile.PossibleTiles);
                    //float noise = UnityEngine.Random.Range(0f, 1f) / 1000f;
                    //if (entropy - noise < minEntropy)
                    if (entropy < minEntropy)
                    {
                        //minEntropy = entropy - noise;
                        minEntropy = entropy;
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
            float randomPoint = UnityEngine.Random.Range(0, 1.0f) * totalWeight;

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
        [NonSerialized] public List<ClassTile> Neighbours;
        public List<ClassTileType> PossibleTiles;
        public bool Collapsed;

        public ClassTile(int x, int y, List<ClassTileType> possibleTiles)
        {
            X = x;
            Y = y;
            Collapsed = false;
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
        public string[] Rules;


        public ClassTileType(string name, int id, int weight = 1)
        {
            Name = name;
            Id = id;
            Weight = weight;
            Rules = new string[4];
        }

        public void AddRule(ClassTileEdge dir, string rule)
        {
            Rules[(int)dir] = rule;
        }
    }
}
