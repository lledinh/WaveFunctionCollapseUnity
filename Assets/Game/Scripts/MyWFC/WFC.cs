using RH;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Build.Content;
using UnityEngine;

namespace Assets.Game.Scripts.MyWFC
{
    public class WFC : MonoBehaviour
    {
        public int Width = 8;
        public int Height = 8;
        public ConfigTile ConfigTile;

        public ClassTile[,] Tiles;

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
                for(int i = 0; i < kv.Value.Count; i++)
                {
                    AllTileTypes.Find(x => x.Name == kv.Key).AddRule((ClassTileEdge) i, AllTileTypes.Find(x => x.Name == kv.Value[i]));
                }
            }

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Tiles[x, y] = new ClassTile(x, y, AllTileTypes);
                }
            }
        }

        public void Run()
        {

        }

        public void Iterate()
        {
            // Get tile with lowest entropy
            (int x, int y) coordsLowestEntropy = GetLowestEntropyTile();
            // Collaspe this tile
            Collapse(coordsLowestEntropy.x, coordsLowestEntropy.y);
            // Propagate the changes
            Propagate(coordsLowestEntropy.x, coordsLowestEntropy.y);
        }

        public void Propagate(int x, int y)
        {
            Stack<ClassTile> tiles = new Stack<ClassTile>();

            tiles.Push(Tiles[x, y]);

            while (tiles.Count > 0)
            {
                ClassTile tile = tiles.Pop();
                // Possible tiles for the current tile
                List<ClassTileType> possibleTiles = tile.PossibleTiles;

                bool constrain = false;
                if (y < Height - 1)
                {
                    ClassTile tileUp = Tiles[x, y + 1];
                    foreach (ClassTileType possibleTile in tileUp.PossibleTiles)
                    {
                        if (possibleTile.Rules[(int)ClassTileEdge.Bottom])
                    }
                }

                constrain = false;
                if (x < Width - 1)
                {
                    ClassTile tileRight = Tiles[x + 1, y];
                }

                constrain = false;
                if (y > 0)
                {
                    ClassTile tileDown = Tiles[x, y - 1];
                }

                constrain = false;
                if (x > 0)
                {
                    ClassTile tileLeft = Tiles[x - 1, y];
                }
            }
        }

        public void Collapse(int x, int y)
        {
            ClassTile tile = Tiles[x, y];
            List<int> weights = new List<int>();
            for (int i = 0; i < tile.PossibleTiles.Count; i++)
            {
                weights[i] = tile.PossibleTiles[i].Weight;
            }

            int rdmChoice = WeightedRandomSelect(weights.ToArray());
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
        public int Entropy;
        public List<ClassTileType> PossibleTiles;

        public ClassTile(int x, int y, List<ClassTileType> possibleTiles)
        {
            X = x;
            Y = y;
            Entropy = -1;
            PossibleTiles = possibleTiles;
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
