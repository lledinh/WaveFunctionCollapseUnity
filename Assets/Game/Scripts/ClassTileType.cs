using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class ClassTileType
{
    // TILE_GRASS    = 0
    // TILE_WATER    = 1
    public string name;
    public int id;

    public ClassTileType(string name, int id)
    {
        this.name = name;
        this.id = id;
    }
}