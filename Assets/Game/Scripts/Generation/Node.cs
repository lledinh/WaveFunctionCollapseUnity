using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TilemapWorldGenerator
{
    [CreateAssetMenu(fileName = "WFCNode", menuName = "WFC/Node")]
    [Serializable]
    public class Node : ScriptableObject
    {
        public Tile Tile;
        public string Name;
        public string ShortName;
        public int Weight;

        public NodeConnection Left;
        public NodeConnection Top;
        public NodeConnection Right;
        public NodeConnection Bottom;

        public Node()
        {
            Left = new NodeConnection();
            Top = new NodeConnection();
            Right = new NodeConnection();
            Bottom = new NodeConnection();
        }
    }
    [Serializable]
    public class NodeConnection
    {
        public List<Node> CompatibleNodes = new List<Node>();
    }
}
