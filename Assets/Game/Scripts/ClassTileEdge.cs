using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class ClassTileEdge
{
    // GRASS    = 0
    // WATER    = 1
    public string name;
    public int id;

    public ClassTileEdge(string name, int id)
    {
        this.name = name;
        this.id = id;
    }
}