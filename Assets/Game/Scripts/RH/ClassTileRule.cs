using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RH
{
    public class ClassTileRule
    {
        public ClassTileType tileType;
        public ClassTileEdge[] edges;

        public ClassTileRule(ClassTileType tileType, ClassTileEdge[] edges)
        {
            this.tileType = tileType;
            this.edges = edges;
        }
    }
}
