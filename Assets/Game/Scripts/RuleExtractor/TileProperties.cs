using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Tilemaps;

namespace TilemapWorldGenerator
{
    public class TileProperties
    {
        public TileBase TileBase { get; private set; }
        public string Name { get; private set; }
        public int Weight { get; private set; }
        public List<TileProperties> Left { get; private set; }
        public List<TileProperties> Top { get; private set; }
        public List<TileProperties> Right { get; private set; }
        public List<TileProperties> Bottom { get; private set; }

        public TileProperties(TileBase tileBase, string name, int weight)
        {
            TileBase = tileBase;
            Name = name;
            Weight = weight;
            Left = new List<TileProperties>();
            Top = new List<TileProperties>();
            Right = new List<TileProperties>();
            Bottom = new List<TileProperties>();
        }

        public void IncreaseWeight(int w)
        {
            Weight += w;
        }
    }
}