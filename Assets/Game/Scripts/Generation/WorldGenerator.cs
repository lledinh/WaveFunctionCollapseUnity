using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TilemapWorldGenerator
{
    public class WorldGenerator : MonoBehaviour
    {
        [SerializeField] private Tilemap Tilemap;
        [SerializeField] private int Width;
        [SerializeField] private int Height;
        public List<Node> Nodes = new List<Node>();
        public Tile BlankTile;
        public Node GrassNode;

        private Node[,] _grid;
        private List<Vector2Int> _toCollapse = new List<Vector2Int>();
        private Vector2Int[] offsets = new Vector2Int[]
        {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0)
        };

        private void InitGrid()
        {
            _grid = new Node[Width, Height];

            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].Name == "Grass")
                {
                    GrassNode = Nodes[i];
                }
            }

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (x == 0 || x == Width - 1 || y == 0 || y == Height - 1)
                    {
                        _grid[x, y] = GrassNode;
                        Tilemap.SetTile(new Vector3Int(x, y, 0), _grid[x, y].Tile);
                    }
                }
            }
        }

        public void Generate()
        {
            string directoryPath = "Assets/Game/ScriptableObjects/Generated";
            Tilemap.ClearAllTiles();
            InitGrid();
            Nodes = Utils.GetAllNodesDefinitionFromDirectory(directoryPath).ToList();
            CollapseWorld();
        }

        private void CollapseWorld()
        {
            _toCollapse.Clear();
            _toCollapse.Add(new Vector2Int(Width / 2, Height / 2));

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    //_toCollapse.Add(new Vector2Int(x, y));
                }
            }
            int iterationCount = 0;

            while (_toCollapse.Count > 0)
            {
                int x = _toCollapse[0].x;
                int y = _toCollapse[0].y;

                List<Node> potentialNodes = new List<Node>(Nodes);

                for (int i = 0; i < offsets.Length; i++)
                {
                    Vector2Int neighbour = new Vector2Int(x + offsets[i].x, y + offsets[i].y);

                    if (IsInsideGrid(neighbour))
                    {
                        Node neighbourNode = _grid[neighbour.x, neighbour.y];

                        if (neighbourNode != null)
                        {
                            switch (i)
                            {
                                case 0:
                                    WhittleNodes(potentialNodes, neighbourNode.Bottom.CompatibleNodes);
                                    break;
                                case 1:
                                    WhittleNodes(potentialNodes, neighbourNode.Top.CompatibleNodes);
                                    break;
                                case 2:
                                    WhittleNodes(potentialNodes, neighbourNode.Left.CompatibleNodes);
                                    break;
                                case 3:
                                    WhittleNodes(potentialNodes, neighbourNode.Right.CompatibleNodes);
                                    break;
                            }
                        }
                        else
                        {
                            if (!_toCollapse.Contains(neighbour))
                            {
                                _toCollapse.Add(neighbour);
                            }
                        }
                    }
                }
                // Propagate
                //* Backtrack essayer tiles restantes si aucune, revenier a la tile généré précedente

                // If cell node has already a tile defined, (it was assigned to the tilemap BEFORE the wave collapse algorithm execution)
                if (_grid[x, y] != null)
                {
                    //Debug.Log($"Not changing tile {x} {y}");
                    _toCollapse.RemoveAt(0);
                    continue;
                }

                if (potentialNodes.Count <= 0)
                {
                    _grid[x, y] = Nodes[0];
                    Tilemap.SetTile(new Vector3Int(x, y, 0), BlankTile);
                    Debug.LogWarning("Attempted to collapse wave on " + x + ", " + y + " but found no compatibles nodes.");
                }
                else
                {
                    _grid[x, y] = potentialNodes[Random.Range(0, potentialNodes.Count)];
                    Tilemap.SetTile(new Vector3Int(x, y, 0), _grid[x, y].Tile);
                }

                _toCollapse.RemoveAt(0);
                iterationCount++;
            }

            Debug.Log($"Iteration count: {iterationCount}");
        }

        private void WhittleNodes(List<Node> potentialNodes, List<Node> validNodes)
        {
            for (int i = potentialNodes.Count - 1; i >= 0; i--)
            {
                if (!validNodes.Contains(potentialNodes[i]))
                {
                    potentialNodes.RemoveAt(i);
                }
            }
        }

        private bool IsInsideGrid(Vector2Int v)
        {
            return v.x >= 0 && v.x < Width && v.y >= 0 && v.y < Height;
        }

    }
}
