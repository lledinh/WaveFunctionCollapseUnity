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
    public class WFC2 : MonoBehaviour
    {
        public int Width = 5;
        public int Height = 5;
        public ConfigTile ConfigTile;

        public ClassTile[,] Tiles;

        public Stack<ClassTile> stackTiles;

        public Tilemap tilemap;
        public Tile[] tiles;
        public Tile errorTile;

        private const int TOP = 0;
        private const int RIGHT = 1;
        private const int BOTTOM = 2;
        private const int LEFT = 3;

        public int seed = -1;
        public bool autorun = true;

        [NonSerialized] public List<ClassTile> TilesList;
        [NonSerialized] public List<ClassTile> TileCollapseOrder;
        private RH.TileDataModel _tileDataModel;
        public int lastSeed = -1;

        public void Start()
        {

            _tileDataModel = ConfigTile.Convert();
            if (autorun) Run();
        }

        private void Init()
        {
            TileCollapseOrder = new List<ClassTile>();
            if (seed == -1)
            {
                seed = UnityEngine.Random.Range(0, 1000000);
            }
            UnityEngine.Random.InitState(seed);
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
                    ClassTileType t = AllTileTypes.Find(x => x.Name == kv.Value[i]);
                    tileType.AddRule((ClassTileEdge)i, t);
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


        public void DrawAll()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (!Tiles[x, y].Collapsed)
                    {
                        Debug.LogError($"{x}, {y} PossibleTiles.Count {Tiles[x, y].PossibleTiles.Count}");
                        tilemap.SetTile(new Vector3Int(x, y, 0), tiles[Tiles[x, y].PossibleTiles[0].Id]);
                    }
                    else
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), tiles[Tiles[x, y].PossibleTiles[0].Id]);
                    }
                }
            }
        }

        bool isRunning = false;
        bool hasInit = false;

        public void Update()
        {
            /*if (!autorun)
            {
                if (!isRunning)
                {
                    if (!hasInit)
                    {
                        Init();
                        Iterate(0, 0);
                        hasInit = true;
                        isRunning = true;
                    }
                }
                else
                {
                    // If space bar input pressed
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        isRunning = !Iterate();
                        DrawAll();
                    }
                }
            }*/
        }

        public void RunRandom()
        {
            Run();
            lastSeed = seed;
            seed = -1;
        }

        public void Run()
        {
            Init();
            Iterate(0, 0);
            bool done = false;
            while (!done)
            {
                done = Iterate();
            }
            DrawAll();
            Debug.Log("Done");
        }

        public bool Iterate(int x = -1, int y = -1)
        {
            if (x == -1 && y == -1)
            {
                (x, y) = GetLowestEntropyTile();
                if (x == -1 &&  (y == -1)) { return true; }
            }
            else
            {
                List<ClassTileType> t = Tiles[x, y].PossibleTiles;
                Tiles[x, y].PossibleTiles = new List<ClassTileType>
                {
                    t[5]
                };
            }
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
                        List<ClassTileType> rulesForCurrentTile = new List<ClassTileType>();
                        if (i == TOP)
                        {
                            // reduced = neighbour.constrain(tilePossibilities, direction)
                            for (int iz = 0; iz < possibleTiles.Count; iz++)
                            {
                                ClassTileType ruleTop = possibleTiles[iz].Rules[TOP];
                                rulesForCurrentTile.Add(ruleTop);
                            }

                            List<ClassTileType> copyNeighbourPossibleTiles = new List<ClassTileType>(neighbour.PossibleTiles);
                            for (int iw = 0; iw < copyNeighbourPossibleTiles.Count; iw++)
                            {
                                // neighbour.PossibleTiles[iw].Rules[2]
                                bool isInRule = false;
                                for (int iy = 0; iy < rulesForCurrentTile.Count; iy++)
                                {
                                    if (copyNeighbourPossibleTiles[iw].Rules[BOTTOM].Id == rulesForCurrentTile[iy].Id)
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

                        rulesForCurrentTile = new List<ClassTileType>();
                        if (i == RIGHT)
                        {
                            // reduced = neighbour.constrain(tilePossibilities, direction)
                            for (int iz = 0; iz < possibleTiles.Count; iz++)
                            {
                                ClassTileType ruleTop = possibleTiles[iz].Rules[RIGHT];
                                rulesForCurrentTile.Add(ruleTop);
                            }

                            List<ClassTileType> copyNeighbourPossibleTiles = new List<ClassTileType>(neighbour.PossibleTiles);
                            for (int iw = 0; iw < copyNeighbourPossibleTiles.Count; iw++)
                            {
                                // neighbour.PossibleTiles[iw].Rules[2]
                                bool isInRule = false;
                                for (int iy = 0; iy < rulesForCurrentTile.Count; iy++)
                                {
                                    if (copyNeighbourPossibleTiles[iw].Rules[LEFT].Id == rulesForCurrentTile[iy].Id)
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

                        rulesForCurrentTile = new List<ClassTileType>();
                        if (i == BOTTOM)
                        {
                            // reduced = neighbour.constrain(tilePossibilities, direction)
                            for (int iz = 0; iz < possibleTiles.Count; iz++)
                            {
                                ClassTileType ruleTop = possibleTiles[iz].Rules[BOTTOM];
                                rulesForCurrentTile.Add(ruleTop);
                            }

                            List<ClassTileType> copyNeighbourPossibleTiles = new List<ClassTileType>(neighbour.PossibleTiles);
                            for (int iw = 0; iw < copyNeighbourPossibleTiles.Count; iw++)
                            {
                                // neighbour.PossibleTiles[iw].Rules[2]
                                bool isInRule = false;
                                for (int iy = 0; iy < rulesForCurrentTile.Count; iy++)
                                {
                                    if (copyNeighbourPossibleTiles[iw].Rules[TOP].Id == rulesForCurrentTile[iy].Id)
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

                        rulesForCurrentTile = new List<ClassTileType>();
                        if (i == LEFT)
                        {
                            // reduced = neighbour.constrain(tilePossibilities, direction)
                            for (int iz = 0; iz < possibleTiles.Count; iz++)
                            {
                                ClassTileType ruleTop = possibleTiles[iz].Rules[LEFT];
                                rulesForCurrentTile.Add(ruleTop);
                            }

                            List<ClassTileType> copyNeighbourPossibleTiles = new List<ClassTileType>(neighbour.PossibleTiles);
                            for (int iw = 0; iw < copyNeighbourPossibleTiles.Count; iw++)
                            {
                                // neighbour.PossibleTiles[iw].Rules[2]
                                bool isInRule = false;
                                for (int iy = 0; iy < rulesForCurrentTile.Count; iy++)
                                {
                                    if (copyNeighbourPossibleTiles[iw].Rules[RIGHT].Id == rulesForCurrentTile[iy].Id)
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
}