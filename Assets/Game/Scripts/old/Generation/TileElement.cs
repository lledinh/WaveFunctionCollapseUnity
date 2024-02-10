using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TilemapWorldGenerator
{
    [CreateAssetMenu(fileName = "TileElement", menuName = "Tilemap Generator/Node")]
    public class TileElement : ScriptableObject
    {
        public UnityEngine.Tilemaps.Tile Tile;
        public string Name;
        public string ShortName;
        public int Weight;

        public TileElementNeighbour Left;
        public TileElementNeighbour Top;
        public TileElementNeighbour Right;
        public TileElementNeighbour Bottom;

        public TileElement()
        {
            Left = new TileElementNeighbour();
            Top = new TileElementNeighbour();
            Right = new TileElementNeighbour();
            Bottom = new TileElementNeighbour();
        }
    }
    [Serializable]
    public class TileElementNeighbour
    {
        public List<TileElement> CompatibleNodes = new List<TileElement>();
    }
}
