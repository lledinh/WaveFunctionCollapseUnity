using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WorldGenerator
{
    [CreateAssetMenu(fileName = "TileConfiguration", menuName = "Tilemap Generator/Tile Configuration")]
    public class TileConfiguration : ScriptableObject
    {
        public string Name;
        public List<TileElement> Tiles = new List<TileElement>();
    }

    [CreateAssetMenu(fileName = "TileElement", menuName = "Tilemap Generator/TileElement")]
    public class TileElement : ScriptableObject
    {
        public Tile Tile;
        public string Name;
        public string ShortName;
        public int Weight;

        public List<TileElement> LeftCompatibleTiles;
        public List<TileElement> TopCompatibleTiles;
        public List<TileElement> RightCompatibleTiles;
        public List<TileElement> BottomCompatibleTiles;

        public TileElement()
        {
            LeftCompatibleTiles = new List<TileElement>();
            TopCompatibleTiles = new List<TileElement>();
            RightCompatibleTiles = new List<TileElement>();
            BottomCompatibleTiles = new List<TileElement>();
        }
    }
}
