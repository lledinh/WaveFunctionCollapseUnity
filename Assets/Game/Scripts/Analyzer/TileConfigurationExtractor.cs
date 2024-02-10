using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TilemapWorldGenerator;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WorldGenerator
{
    public class TileConfigurationExtractor : MonoBehaviour
    {
        [SerializeField] private Tilemap Tilemap;

        private TileElement[] RetrieveTilesTypes(TileBase[] tiles)
        {
            List<TileElement> tilesTypes = new List<TileElement>();
            for (int i = 0; i < tiles.Length; i++)
            {
                TileBase tile = tiles[i];
                TileElement t = tilesTypes.FirstOrDefault(tileType => tileType.Tile as TileBase == tile);
                if (t == null)
                {
                    tilesTypes.Add(new TileElement());
                }
                else
                {
                    t.Weight += 1;
                }
            }

            return tilesTypes.ToArray();
        }

    }
}
