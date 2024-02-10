using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Tilemaps;
using UnityEngine;
using TilemapWorldGenerator;
using System.Collections;
using UnityEditor;
using Unity.Mathematics;

namespace WFCEditor
{
    public class WFCLiveRendering : MonoBehaviour
    {
        [Serializable]
        public class WFCNode
        {
            public Vector2Int Coords;
            public List<TileElement> PossibleNodes;
            public HashSet<TileElement> TriedNodes;
            public TileElement ChosenNode;
        }

        public class StepData
        {

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

        public void Initalize()
        {
            Tilemap.ClearAllTiles();
            NodesDefinitionList = Utils.GetAllNodesDefinitionFromDirectory("Assets/Game/ScriptableObjects/Generated").ToList();

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
            nodeProcessQueue.Add(GetFirstNodeToProcess());
        }

        public Vector2Int GetFirstNodeToProcess()
        {
            return new Vector2Int(Width / 2, Height / 2);
        }

        public void GenerateStep()
        {
            Initalize();
            Step();
        }
        public void GenerateStepLinear()
        {
            Initalize();
            nodeProcessQueue.Clear();
            for (int j = 0; j < Height; j++)
            {
                for (int i = 0; i < Width; i++)
                {
                    nodeProcessQueue.Add(new Vector2Int(i, j));
                }
            }
            //StepLinear();
        }

        public bool ValidCoordinates(Vector2Int v)
        {
            return (v.x >= 0 && v.x < Width && v.y >= 0 && v.y < Height);
        }

        public Vector2Int[] GetNeighboursCoords(Vector2Int nodeCoords)
        {
            return new Vector2Int[] 
            {
                    new Vector2Int(nodeCoords.x - 1, nodeCoords.y),
                    new Vector2Int(nodeCoords.x, nodeCoords.y + 1),
                    new Vector2Int(nodeCoords.x + 1, nodeCoords.y),
                    new Vector2Int(nodeCoords.x, nodeCoords.y - 1),
            };
        }

        public void ResetNode(WFCNode node)
        {
            node.PossibleNodes = new List<TileElement>(NodesDefinitionList);
            node.TriedNodes = new HashSet<TileElement>();
            node.ChosenNode = null;
            Tilemap.SetTile(new Vector3Int(node.Coords.x, node.Coords.y, 0), BlankTile);
        }


        public bool StepLinear()
        {
            if (nodeProcessQueue.Count > 0)
            {
                Vector2Int nodeCoords = nodeProcessQueue.First();
                nodeProcessQueue.RemoveAt(0);
                WFCNode node = _grid[nodeCoords.x, nodeCoords.y];

                if (node.PossibleNodes.Count <= 0)
                {
                    // No possible state for current node, reset it
                    ResetNode(node);

                    WFCNode previous = nodeHistory.Pop();
                    previous.PossibleNodes.Remove(previous.ChosenNode);
                    previous.TriedNodes.Add(previous.ChosenNode);
                    nodeProcessQueue.Insert(0, node.Coords);
                    nodeProcessQueue.Insert(0, previous.Coords);
                    return false;
                }

                Vector2Int[] neighboursCoords = GetNeighboursCoords(nodeCoords);

                WFCNode leftNode = ValidCoordinates(neighboursCoords[0]) ? _grid[neighboursCoords[0].x, neighboursCoords[0].y] : null;
                WFCNode topNode = ValidCoordinates(neighboursCoords[1]) ? _grid[neighboursCoords[1].x, neighboursCoords[1].y] : null;
                WFCNode rightNode = ValidCoordinates(neighboursCoords[2]) ? _grid[neighboursCoords[2].x, neighboursCoords[2].y] : null;
                WFCNode bottomNode = ValidCoordinates(neighboursCoords[3]) ? _grid[neighboursCoords[3].x, neighboursCoords[3].y] : null;

                if (leftNode != null)
                {
                    if (leftNode.ChosenNode != null)
                    {
                        ReduceNodes(node, leftNode.ChosenNode.Right.CompatibleNodes);
                    }
                }

                if (topNode != null)
                {
                    if (topNode.ChosenNode != null)
                    {
                        ReduceNodes(node, topNode.ChosenNode.Bottom.CompatibleNodes);
                    }
                }

                if (rightNode != null)
                {
                    if (rightNode.ChosenNode != null)
                    {
                        ReduceNodes(node, rightNode.ChosenNode.Left.CompatibleNodes);
                    }
                }

                if (bottomNode != null)
                {
                    if (bottomNode.ChosenNode != null)
                    {
                        ReduceNodes(node, bottomNode.ChosenNode.Top.CompatibleNodes);
                    }
                }


                if (node.PossibleNodes.Count <= 0)
                {
                    // No possible state for current node, reset it
                    ResetNode(node);

                    WFCNode previous = nodeHistory.Pop();
                    previous.PossibleNodes.Remove(previous.ChosenNode);
                    previous.TriedNodes.Add(previous.ChosenNode);
                    nodeProcessQueue.Insert(0, node.Coords);
                    nodeProcessQueue.Insert(0, previous.Coords);
                }
                else
                {
                    ChooseRandomNodeWeighted(node);
                    nodeHistory.Push(node);
                }
                return false;
            }
            return true;
        }

        public bool Step()
        {
            if (nodeProcessQueue.Count > 0)
            {
                Vector2Int nodeCoords = nodeProcessQueue.LastOrDefault();
                nodeProcessQueue.RemoveAt(nodeProcessQueue.Count() - 1);
                WFCNode node = _grid[nodeCoords.x, nodeCoords.y];

                if (node.PossibleNodes.Count <= 0)
                {
                    // No possible state for current node, reset it
                    Debug.LogWarning($"wfcNode no possible nodes {node.Coords.x} {node.Coords.y}.");
                    ResetNode(node);

                    WFCNode previous = nodeHistory.Pop();
                    Debug.Log($"Before: {previous.Coords.x} {previous.Coords.y} " + previous.PossibleNodes.Count);
                    previous.PossibleNodes.Remove(previous.ChosenNode);
                    Debug.Log($"After: {previous.Coords.x} {previous.Coords.y} " + previous.PossibleNodes.Count);
                    previous.TriedNodes.Add(previous.ChosenNode);
                    nodeProcessQueue.Add(previous.Coords);
                    return false;
                }


                Debug.Log($"Node: {node.Coords.x} {node.Coords.y} " + node.PossibleNodes.Count);
                Vector2Int[] neighboursCoords = GetNeighboursCoords(nodeCoords);

                WFCNode leftNode = ValidCoordinates(neighboursCoords[0]) ? _grid[neighboursCoords[0].x, neighboursCoords[0].y] : null;
                WFCNode topNode = ValidCoordinates(neighboursCoords[1]) ? _grid[neighboursCoords[1].x, neighboursCoords[1].y] : null;
                WFCNode rightNode = ValidCoordinates(neighboursCoords[2]) ? _grid[neighboursCoords[2].x, neighboursCoords[2].y] : null;
                WFCNode bottomNode = ValidCoordinates(neighboursCoords[3]) ? _grid[neighboursCoords[3].x, neighboursCoords[3].y] : null;

                if (leftNode != null)
                {
                    if (leftNode.ChosenNode != null)
                    {
                        ReduceNodes(node, leftNode.ChosenNode.Right.CompatibleNodes);
                    }
                    //else if (!nodeProcessQueue.Contains(leftNode.Coords))
                    else
                    {
                        nodeProcessQueue.Add(leftNode.Coords);
                    }
                }

                if (topNode != null)
                {
                    if (topNode.ChosenNode != null)
                    {
                        ReduceNodes(node, topNode.ChosenNode.Bottom.CompatibleNodes);
                    }
                    // else if (!nodeProcessQueue.Contains(topNode.Coords))
                    else {
                        nodeProcessQueue.Add(topNode.Coords);
                    }
                }

                if (rightNode != null)
                {
                    if (rightNode.ChosenNode != null)
                    {
                        ReduceNodes(node, rightNode.ChosenNode.Left.CompatibleNodes);
                    }
                    // else if (!nodeProcessQueue.Contains(rightNode.Coords))
                    else {
                        nodeProcessQueue.Add(rightNode.Coords);
                    }
                }

                if (bottomNode != null)
                {
                    if (bottomNode.ChosenNode != null)
                    {
                        ReduceNodes(node, bottomNode.ChosenNode.Top.CompatibleNodes);
                    }
                    // else if (!nodeProcessQueue.Contains(bottomNode.Coords))
                    else {
                        nodeProcessQueue.Add(bottomNode.Coords);
                    }
                }

                if (node.TriedNodes.Count >= NodesDefinitionList.Count)
                {
                    Debug.LogWarning($"node TriedNodes max {node.Coords.x} {node.Coords.y}.");
                }

                if (node.PossibleNodes.Count <= 0)
                {
                    // No possible state for current node, reset it
                    Debug.LogWarning($"end wfcNode no possible nodes {node.Coords.x} {node.Coords.y}.");
                    ResetNode(node);

                    WFCNode previous = nodeHistory.Pop();
                    Debug.Log($"Before: {previous.Coords.x} {previous.Coords.y} " + previous.PossibleNodes.Count);
                    previous.PossibleNodes.Remove(previous.ChosenNode);
                    Debug.Log($"After: {previous.Coords.x} {previous.Coords.y} " + previous.PossibleNodes.Count);
                    previous.TriedNodes.Add(previous.ChosenNode);
                    nodeProcessQueue.Add(previous.Coords);
                }
                else
                {
                    ChooseRandomNode(node);
                    nodeHistory.Push(node);
                }
                return false;
            }
            return true;
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
            _grid[wfcNode.Coords.x, wfcNode.Coords.y].ChosenNode = wfcNode.PossibleNodes[UnityEngine.Random.Range(0, wfcNode.PossibleNodes.Count)];
            wfcNode.ChosenNode = _grid[wfcNode.Coords.x, wfcNode.Coords.y].ChosenNode;
            UpdateTilemap(wfcNode);
        }


        public void ChooseRandomNodeWeighted(WFCNode wfcNode)
        {
            int totalWeight = 0;
            foreach (var n in wfcNode.PossibleNodes)
            {
                totalWeight += n.Weight;
            }

            System.Random random = new System.Random();
            int randomNumber = random.Next(totalWeight);
            int sum = 0;

            foreach (var n in wfcNode.PossibleNodes)
            {
                sum += n.Weight;
                if (randomNumber < sum)
                {
                    _grid[wfcNode.Coords.x, wfcNode.Coords.y].ChosenNode = n;
                    wfcNode.ChosenNode = _grid[wfcNode.Coords.x, wfcNode.Coords.y].ChosenNode;
                    UpdateTilemap(wfcNode);
                    break;
                }
            }
        }

        public void UpdateTilemap(WFCNode wfcNode)
        {
            Tilemap.SetTile(new Vector3Int(wfcNode.Coords.x, wfcNode.Coords.y, 0), _grid[wfcNode.Coords.x, wfcNode.Coords.y].ChosenNode.Tile);
        }

        public void RandomTile()
        {
            Debug.Log("New random tile...");
            UnityEngine.Tilemaps.Tile tile = NodesDefinitionList[UnityEngine.Random.Range(0, NodesDefinitionList.Count())].Tile;
            Tilemap.SetTile(new Vector3Int(0, 0, 0), tile);
        }




        /*if (leftNode != null)
            if (leftNode.ChosenNode != null)
                ReduceNodes(node, leftNode.ChosenNode.Right.CompatibleNodes);
            else
                nodeProcessQueue.Add(leftNode.Coords);

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
                traversalNodes.Add(bottomNode);*/
    }
}
