using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ClassTileWeight
{
    public ClassTileType type;
    public int weight;

    public ClassTileWeight(ClassTileType type, int weight)
    {
        this.type = type;
        this.weight = weight;
    }
}