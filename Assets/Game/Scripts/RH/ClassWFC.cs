using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RH
{
    [Serializable]
    public class ClassWFC
    {
        public ClassTile[,] Tiles;

        public ClassWFC(int width, int height, TileDataModel tileDataModel)
        {
            Tiles = new ClassTile[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Tiles[x, y] = new ClassTile(x, y, tileDataModel.TileTypes, tileDataModel.TileWeights);
                }
            }
        }

        public ClassTile Get(int x, int y)
        {
            return Tiles[x, y];
        }

        public bool IsFullyCollapsed()
        {
            return false;
        }

        public void GetAllCollapsed()
        {

        }

        public void Collapse(int x, int y)
        {
            ClassTile tile = Tiles[x, y];
            Dictionary<string, int> possibleTiles = tile.PossibleTiles;
            Dictionary<string, int> tilesWeights = tile.TilesWeights;

            Dictionary<string, int> filteredTilesWeights = new Dictionary<string, int>();
            foreach (var tileEntry in possibleTiles)
            {
                string tileKey = tileEntry.Key;
                if (tilesWeights.ContainsKey(tileKey))
                {
                    filteredTilesWeights[tileKey] = tilesWeights[tileKey];
                }
            }

            // Check array order
            int rdmChoice = WeightedRandomSelect(filteredTilesWeights.Values.ToArray());
            tile.PossibleTiles.Clear();
            var k = filteredTilesWeights.Keys.ToArray()[rdmChoice];
            var v = filteredTilesWeights.Values.ToArray()[rdmChoice];
            tile.PossibleTiles.Add(k, v);
        }

        public void Constrain(int x, int y, string forbiddenTile)
        {
            ClassTile tile = Tiles[x, y];
            tile.PossibleTiles.Remove(forbiddenTile);
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
