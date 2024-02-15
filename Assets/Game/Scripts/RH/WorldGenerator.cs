using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RH
{
    public class WorldGenerator : MonoBehaviour
    {
        public ConfigTile config;
        public JSONTileDataModel JSONtileDataModel;
        public TileDataModel tileDataModel;
        public ClassWFC wfc;

        void Start()
        {
            JSONtileDataModel = config.jsonTileDataModel;
            tileDataModel = config.Convert();
        }

        void Generate()
        {

        }
    }
}
