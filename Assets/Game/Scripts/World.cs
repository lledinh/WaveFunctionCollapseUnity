using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class World : MonoBehaviour
{
    public int Width = 8;
    public int Height = 8;

    public List<ClassTile> DebugTiles;
    public ClassTile[,] Tiles;
    public Config config;
    [SerializeField] public ConfigReader _configReader;
    public ClassTileConfiguration classTileConfiguration;

    [ContextMenu("Start initialization")]
    void Start()
    {
        TileDataModel TileDataModel = _configReader.ReadJSON();
        Debug.Log(TileDataModel.TileTypes.Count);

        classTileConfiguration = new ClassTileConfiguration(TileDataModel);

        Tiles = new ClassTile[Width, Height];
        DebugTiles = new List<ClassTile>();
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Tiles[x, y] = new ClassTile(x, y, classTileConfiguration.TileTypes);
                DebugTiles.Add(Tiles[x, y]);
            }
        }

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                ClassTile tile = Tiles[x, y];
                // NORTH
                if (y > 0)
                {
                    tile.AddNeighbour(0, Tiles[x, y - 1]);
                    //tile.AddNeighbour(0, Tiles[x, y + 1]);
                }
                // EAST
                if (x < Width - 1)
                {
                    tile.AddNeighbour(1, Tiles[x + 1, y]);
                }
                // SOUTH
                if (y < Height - 1)
                {
                    tile.AddNeighbour(2, Tiles[x, y + 1]);
                    //tile.AddNeighbour(0, Tiles[x, y - 1]);
                }
                // WEST
                if (x > 0)
                {
                    tile.AddNeighbour(3, Tiles[x - 1, y]);
                }
            }
        }
        Debug.Log("Initialization complete");
        Debug.Log("Starting WFC");
        WaveFunctionCollapse();
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (Tiles[x, y].Entropy < 10)
                {
                    Debug.Log($"Tile {x}, {y} has entropy {Tiles[x, y].Entropy}");
                }
            }
        }
    }

    public int GetEntropy(int x, int y)
    {
        return Tiles[x, y].Entropy;
    }

    // Get tile type
    public ClassTileType GetTileType(int x, int y)
    {
        return Tiles[x, y].Possibilities[0];
    }

    // Get lowest entropy
    public ClassTile GetLowestEntropyTile()
    {
        ClassTile lowestEntropyTile = null;
        int lowestEntropy = int.MaxValue;

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (Tiles[x, y].Entropy > 0 && Tiles[x, y].Entropy < lowestEntropy)
                {
                    lowestEntropy = Tiles[x, y].Entropy;
                    lowestEntropyTile = Tiles[x, y];
                }
            }
        }

        return lowestEntropyTile;
    }

    // Get list oif tiles with lowest entropy
    public List<ClassTile> GetLowestEntropyTiles()
    {
        List<ClassTile> lowestEntropyTiles = new List<ClassTile>();
        int lowestEntropy = int.MaxValue;

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (Tiles[x, y].Entropy > 0 && Tiles[x, y].Entropy < lowestEntropy)
                {
                    lowestEntropy = Tiles[x, y].Entropy;
                }
            }
        }

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (Tiles[x, y].Entropy == lowestEntropy)
                {
                    lowestEntropyTiles.Add(Tiles[x, y]);
                }
            }
        }

        return lowestEntropyTiles;
    }

    public void WaveFunctionCollapse()
    {
        List<ClassTile> tilesLowestEntropy = GetLowestEntropyTiles();

        if (tilesLowestEntropy == null || tilesLowestEntropy.Count <= 0)
        {
            Debug.LogError("No tiles with entropy");
        }

        // Get a random tile to collapse among tilesLowestEntropy
        ClassTile tileToCollapse = tilesLowestEntropy[UnityEngine.Random.Range(0, tilesLowestEntropy.Count)];
        // Collapse the tile
        tileToCollapse.Collapse(classTileConfiguration.TileWeights);
        // Add tile to collapse to a stack
        Stack<ClassTile> stack = new Stack<ClassTile>();
        stack.Push(tileToCollapse);

        Debug.Log($"Initial Tile {tileToCollapse.X}, {tileToCollapse.Y} chosen.");

        int failSafeIndex = 0;
        // While stack is not empty
        List<ClassTile> processedTiles = new List<ClassTile>();
        while (stack.Count > 0)
        {
            Debug.Log($"i {failSafeIndex}");
            if (failSafeIndex > 5000)
            {
                Debug.LogError("Fail safe triggered");
                break;
            }
            ClassTile tile = stack.Pop();
            processedTiles.Add(tile);
            // Get tiles possibilities
            List<ClassTileType> possibilities = tile.Possibilities;
            // Get neighbours
            ClassTile[] neighbours = tile.Neighbours;

            for (int i = 0; i < neighbours.Length; i++)
            {
                ClassTile neighbour = neighbours[i];
                if (neighbour != null && neighbour.Entropy != 0)
                {
                    bool reduced = neighbour.Constrain(possibilities, i, classTileConfiguration.Rules);
                    if (reduced)
                    {
                        stack.Push(neighbour);
                    }
                }
            }
            failSafeIndex++;
        }
    }

    void Update()
    {
        
    }
}
