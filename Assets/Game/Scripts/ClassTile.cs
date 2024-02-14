using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.tvOS;
using UnityEngine.UIElements;

[Serializable]
public class ClassTile
{
    public int X;
    public int Y;
    public int Entropy;
    private List<ClassTileType> possibilities;
    private ClassTile[] neighbours;

    public List<ClassTileType> Possibilities { get => possibilities; set => possibilities = value; }
    public ClassTile[] Neighbours { get => neighbours; set => neighbours = value; }

    public ClassTile(int x, int y, List<ClassTileType> allPossibilities)
    {
        X = x;
        Y = y;
        Neighbours = new ClassTile[4];
        Possibilities = new List<ClassTileType>(allPossibilities);
        //Possibilities.AddRange(possibilities);
        Entropy = Possibilities.Count;
    }

    public void AddNeighbour(int direction, ClassTile tile)
    {
        Neighbours[direction] = tile;
    }

    public ClassTile GetNeighbour(int direction)
    {
        return Neighbours[direction];
    }

    public void Collapse(Dictionary<ClassTileType, int> TileWeights)
    {
        List<int> weights = new List<int>();
        for(int i = 0; i < Possibilities.Count; i++)
        {
            weights.Add(TileWeights[Possibilities[i]]);
        }

        int rdmChoice = WeightedRandomSelect(weights.ToArray());
        possibilities = new List<ClassTileType> { Possibilities[rdmChoice] };
        Entropy = 0;
    }

    public bool Constrain(List<ClassTileType> neighbourPossibilities, int direction, List<ClassTileRule> Rules)
    {
        Debug.Log($"Constrain tile {X} {Y}");
        bool reduced = false;

        if (Entropy > 0)
        {
            List<ClassTileEdge> connectors = new List<ClassTileEdge>();

            for (int i = 0; i < neighbourPossibilities.Count; i++)
            {
                connectors.Add(Rules.Find(r => r.tileType.id == neighbourPossibilities[i].id).edges[direction]);
            }

            int checkDirection = -1;
            if (direction == 0) checkDirection = 2;
            if (direction == 1) checkDirection = 3;
            if (direction == 2) checkDirection = 0;
            if (direction == 3) checkDirection = 1;

            List<ClassTileType> copyPossibilities = new List<ClassTileType>();
            for (int i = 0; i < Possibilities.Count; i++)
            {
                ClassTileType c = Possibilities[i];
                copyPossibilities.Add(new ClassTileType(c.name, c.id));
            }

            for (int i = 0; i < copyPossibilities.Count; i++)
            {
                ClassTileEdge edge = Rules.Find(r => r.tileType.id == copyPossibilities[i].id).edges[checkDirection];
                if (connectors.Find(r => r.id == edge.id) == null)
                {
                    ClassTileType itemToRemove = possibilities.Find(item => item.id == copyPossibilities[i].id);
                    bool removed = false;
                    if (itemToRemove != null)
                    {
                        removed = possibilities.Remove(itemToRemove);
                        Debug.Log($"removed {removed}");
                    }
                    reduced = true;
                }
            }

            Entropy = possibilities.Count;
        }

        return reduced;
    }

    int WeightedRandomSelect(int[] weights)
    {
        float totalWeight = 0f;
        for (int i = 0; i < weights.Length; i++)
        {
            totalWeight += weights[i];
        }

        float randomPoint = UnityEngine.Random.value * totalWeight;

        for (int i = 0; i < weights.Length; i++)
        {
            if (randomPoint < weights[i])
            {
                return i;
            }
            randomPoint -= weights[i];
        }

        return -1; // Should not happen
    }
}