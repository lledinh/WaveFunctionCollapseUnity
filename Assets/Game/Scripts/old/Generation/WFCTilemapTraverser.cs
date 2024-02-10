using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using static TilemapWorldGenerator.WFCTilemapTraverser;
using Random = UnityEngine.Random;

namespace TilemapWorldGenerator
{
    public class WFCTilemapTraverser : MonoBehaviour
    {
        [Serializable]
        public class WFCNode
        {
            public int x;
            public int y;
            public List<TileElement> PossibleNodes;
            public HashSet<TileElement> TriedNodes;
            public TileElement ChosenNode;
        }

        private List<TileElement> NodesDefinitionList;
        public List<WFCNode> traversalNodes;
        public Stack<WFCNode> traversalHistory;
        public WFCNode[,] _grid;

        public int Width;
        public int Height;
        [SerializeField] private Tilemap Tilemap;

        public UnityEngine.Tilemaps.Tile BlankTile;
        public TileElement GrassNode;


        public void InitGrid()
        {
            traversalNodes = new List<WFCNode>();
            _grid = new WFCNode[Width, Height];
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    _grid[i, j] = new WFCNode();
                    _grid[i, j].x = i;
                    _grid[i, j].y = j;
                    _grid[i, j].PossibleNodes = new List<TileElement>(NodesDefinitionList);
                    _grid[i, j].TriedNodes = new HashSet<TileElement>();
                }
            }
        }

        public void Generate()
        {
            string directoryPath = "Assets/Game/ScriptableObjects/Generated";
            Tilemap.ClearAllTiles();
            NodesDefinitionList = Utils.GetAllNodesDefinitionFromDirectory(directoryPath).ToList();
            InitGrid();
            Traverse2();
        }

        public void Traverse2()
        {
            int maxIteration = 10000;
            int iteration = 0;

            traversalHistory = new Stack<WFCNode>();
            traversalNodes.Add(_grid[Width / 2, Height / 2]);

            while (traversalNodes.Count > 0 && iteration < maxIteration)
            {
                Debug.Log($"Iteration {iteration}");
                WFCNode wfcNode = traversalNodes.LastOrDefault();
                traversalNodes.RemoveAt(traversalNodes.Count() - 1);

                WFCNode[] neighbours = {
                    (wfcNode.x - 1 > 0) ? _grid[wfcNode.x - 1, wfcNode.y] : null,
                    (wfcNode.y + 1 < Height) ? _grid[wfcNode.x, wfcNode.y + 1] : null,
                    (wfcNode.x + 1 < Width) ? _grid[wfcNode.x + 1, wfcNode.y] : null,
                    (wfcNode.y - 1 > 0) ? _grid[wfcNode.x, wfcNode.y - 1] : null,
                };

                WFCNode leftNode = neighbours[0];
                WFCNode topNode = neighbours[1];
                WFCNode rightNode = neighbours[2];
                WFCNode bottomNode = neighbours[3];

                if (leftNode != null)
                    if (leftNode.ChosenNode != null)
                        ReduceNodes(wfcNode, leftNode.ChosenNode.Right.CompatibleNodes);
                    else
                        traversalNodes.Add(leftNode);

                if (topNode != null)
                    if (topNode.ChosenNode != null)
                        ReduceNodes(wfcNode, topNode.ChosenNode.Bottom.CompatibleNodes);
                    else
                        traversalNodes.Add(topNode);

                if (rightNode != null)
                    if (rightNode?.ChosenNode != null)
                        ReduceNodes(wfcNode, rightNode.ChosenNode.Left.CompatibleNodes);
                    else
                        traversalNodes.Add(rightNode);

                if (bottomNode != null)
                    if (bottomNode?.ChosenNode != null)
                        ReduceNodes(wfcNode, bottomNode.ChosenNode.Top.CompatibleNodes);
                    else
                        traversalNodes.Add(bottomNode);

                if (wfcNode.PossibleNodes.Count <= 0)
                {
                    // No possible state for current node, reset it
                    Debug.LogWarning($"wfcNode no possible nodes {wfcNode.x} {wfcNode.y}.");
                    wfcNode.PossibleNodes = new List<TileElement>(NodesDefinitionList);
                    wfcNode.TriedNodes = new HashSet<TileElement>();
                    wfcNode.ChosenNode = null;

                    Debug.Log("before pop " + traversalHistory.Count());
                    WFCNode previous = traversalHistory.Pop();
                    Debug.Log("after pop " + traversalHistory.Count());
                    previous.PossibleNodes.Remove(previous.ChosenNode);
                    previous.TriedNodes.Add(previous.ChosenNode);
                    traversalNodes.Add(previous);
                    break;
                }
                else
                {
                    ChooseRandomNode(wfcNode);
                    traversalHistory.Push(wfcNode);
                }

                iteration++;
            }
        }


        public void Traverse()
        {
            traversalHistory = new Stack<WFCNode>();
            traversalNodes.Add(_grid[Width / 2, Height / 2]);

            // Failsafe : use time or value
            int maxIteration = 10000;
            int iteration = 0;
            WFCNode previous = null;
            Debug.Log("Start");
            while (traversalNodes.Count > 0 && iteration < maxIteration)
            {
                Debug.Log($"Iteration {iteration}");
                WFCNode wfcNode = traversalNodes.LastOrDefault();
                List<TileElement> initialPossibleNodes = new List<TileElement>(wfcNode.PossibleNodes);
                traversalNodes.RemoveAt(traversalNodes.Count() - 1);
                // Left Top Right Bottom
                WFCNode[] neighbours = {
                    (wfcNode.x - 1 > 0) ? _grid[wfcNode.x - 1, wfcNode.y] : null,
                    (wfcNode.y + 1 < Height) ? _grid[wfcNode.x, wfcNode.y + 1] : null,
                    (wfcNode.x + 1 < Width) ? _grid[wfcNode.x + 1, wfcNode.y] : null,
                    (wfcNode.y - 1 > 0) ? _grid[wfcNode.x, wfcNode.y - 1] : null,
                };

                WFCNode leftNode = neighbours[0];
                WFCNode topNode = neighbours[1];
                WFCNode rightNode = neighbours[2];
                WFCNode bottomNode = neighbours[3];
                // Randomize neighbour added order
                if (leftNode != null)
                    if (leftNode.ChosenNode != null)
                        ReduceNodes(wfcNode, leftNode.ChosenNode.Right.CompatibleNodes);
                    else
                        traversalNodes.Add(leftNode);

                if (topNode != null)
                    if (topNode.ChosenNode != null)
                        ReduceNodes(wfcNode, topNode.ChosenNode.Bottom.CompatibleNodes);
                    else
                        traversalNodes.Add(topNode);

                if (rightNode != null)
                    if (rightNode?.ChosenNode != null)
                        ReduceNodes(wfcNode, rightNode.ChosenNode.Left.CompatibleNodes);
                    else
                        traversalNodes.Add(rightNode);

                if (bottomNode != null)
                    if (bottomNode?.ChosenNode != null)
                        ReduceNodes(wfcNode, bottomNode.ChosenNode.Top.CompatibleNodes);
                    else
                        traversalNodes.Add(bottomNode);

                // The tile chosen previously lead to a dead end, delete it from the previous list of choice
                // ALSO CHECK IF ALL NODES TRIED!!!!!!
                if (wfcNode.TriedNodes.Count() >= NodesDefinitionList.Count())
                {
                    // No possible state for current node, reset it
                    Debug.LogWarning($"wfcNode has tried all nodes {wfcNode.x} {wfcNode.y}.");
                    wfcNode.PossibleNodes = new List<TileElement>(NodesDefinitionList);
                    wfcNode.TriedNodes = new HashSet<TileElement>();
                    wfcNode.ChosenNode = null;

                    previous = traversalHistory.Pop();
                    previous.PossibleNodes.Remove(previous.ChosenNode);
                    previous.TriedNodes.Add(previous.ChosenNode);
                    traversalNodes.Add(previous);
                }
                else if (wfcNode.PossibleNodes.Count <= 0)
                {
                    // No possible state for current node, reset it
                    Debug.LogWarning($"wfcNode no possible nodes {wfcNode.x} {wfcNode.y}.");
                    wfcNode.PossibleNodes = new List<TileElement>(NodesDefinitionList);
                    wfcNode.TriedNodes = new HashSet<TileElement>();
                    wfcNode.ChosenNode = null;

                    previous = traversalHistory.Pop();
                    previous.PossibleNodes.Remove(previous.ChosenNode);
                    previous.TriedNodes.Add(previous.ChosenNode);
                    traversalNodes.Add(previous);
                    break;
                }
                else
                {
                    ChooseRandomNode(wfcNode);
                    traversalHistory.Push(wfcNode);
                }

                iteration++;
            }


            if (iteration > maxIteration)
            {
                Debug.LogError($"Max Iteration of {maxIteration} reached.");
            }
            Debug.Log("End");
        }

        private void ReduceNodes(WFCNode wfcNode, List<TileElement> validNodes)
        {
            for (int i = wfcNode.PossibleNodes.Count - 1; i >= 0; i--)
            {
                if (!validNodes.Contains(wfcNode.PossibleNodes[i]))
                {
                    wfcNode.PossibleNodes.RemoveAt(i);
                }
            }
        }

        public void ChooseRandomNode(WFCNode wfcNode)
        {
            _grid[wfcNode.x, wfcNode.y].ChosenNode = wfcNode.PossibleNodes[Random.Range(0, wfcNode.PossibleNodes.Count)];
            wfcNode.ChosenNode = _grid[wfcNode.x, wfcNode.y].ChosenNode;
            UpdateTilemap(wfcNode);
        }

        public void UpdateTilemap(WFCNode wfcNode)
        {
            Tilemap.SetTile(new Vector3Int(wfcNode.x, wfcNode.y, 0), _grid[wfcNode.x, wfcNode.y].ChosenNode.Tile);
        }

        public void Apply()
        {

        }

        public void Rollback()
        {

        }
    }
}