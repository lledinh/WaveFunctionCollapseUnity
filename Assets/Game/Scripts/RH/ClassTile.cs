using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RH
{
    public class ClassTile
    {
        public int X;
        public int Y;
        public int Entropy;
        private Dictionary<string, int> possibleTiles;
        private Dictionary<string, int> tilesWeights;
        private ClassTile[] neighbours;

        public Dictionary<string, int> PossibleTiles { get => possibleTiles; set => possibleTiles = value; }
        public Dictionary<string, int> TilesWeights { get => tilesWeights; set => tilesWeights = value; }
        public ClassTile[] Neighbours { get => neighbours; set => neighbours = value; }

        public ClassTile(int x, int y, Dictionary<string, int> allPossibleTiles, Dictionary<string, int> tilesWeights)
        {
            X = x;
            Y = y;
            Neighbours = new ClassTile[4];
            PossibleTiles = new Dictionary<string, int>(allPossibleTiles);
            TilesWeights = new Dictionary<string, int>(tilesWeights);
            Entropy = PossibleTiles.Count;
        }

    }
}