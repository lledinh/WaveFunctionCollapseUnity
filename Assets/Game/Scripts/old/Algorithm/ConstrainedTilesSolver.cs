using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TilemapWorldGenerator;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ConstrainedTilesSolver
{
    [Serializable]
    public class WFCNode
    {
        public Vector2Int Coords;
        public List<TileElement> PossibleNodes;
        public HashSet<TileElement> TriedNodes;
        public TileElement ChosenNode;
    }

    // Nodes definition (Name, tile, rules...)
    public List<TileElement> NodesDefinitionList;

    // Nodes to be processed by the algorithm
    public List<Vector2Int> nodeProcessQueue;
    // Node history used to backtrack if necessary
    public Stack<WFCNode> nodeHistory;
    // Nodes grid
    public WFCNode[,] _grid;

    // Tilemap Width
    public int Width;
    // Tilemap Height
    public int Height;
    // The tilemap where the generated map will be outputed 
    [SerializeField] private Tilemap Tilemap;

    // Blank tile for debugging purpose
    public UnityEngine.Tilemaps.Tile BlankTile;
    public TileElement GrassNode;

    // Failsafe to avoid potential infinite loop
    public int maxIteration = 10000;
    public int iteration = 0;

    public void SetupTilemap()
    {
        Tilemap.ClearAllTiles();
    }

    public void RetrieveTilesElement()
    {
        NodesDefinitionList = Utils.GetAllNodesDefinitionFromDirectory("Assets/Game/ScriptableObjects/Generated").ToList();
    }

    public void SetupGrid()
    {
        _grid = new WFCNode[Width, Height];
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                _grid[i, j] = new WFCNode();

                _grid[i, j].Coords = new Vector2Int(i, j);
                _grid[i, j].PossibleNodes = new List<TileElement>(NodesDefinitionList);
                _grid[i, j].TriedNodes = new HashSet<TileElement>();
            }
        }
    }
    
    public void Initalize()
    {
        SetupTilemap();
        RetrieveTilesElement();
        nodeProcessQueue = new List<Vector2Int>();
        nodeHistory = new Stack<WFCNode>();

        _grid = new WFCNode[Width, Height];
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                _grid[i, j] = new WFCNode();

                _grid[i, j].Coords = new Vector2Int(i, j);
                _grid[i, j].PossibleNodes = new List<TileElement>(NodesDefinitionList);
                _grid[i, j].TriedNodes = new HashSet<TileElement>();
            }
        }
    }
}