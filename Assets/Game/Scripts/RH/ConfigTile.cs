using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RH
{
    [CreateAssetMenu(fileName = "ConfigTile", menuName = "ConfigTile")]
    public class ConfigTile : ScriptableObject
    {
        [SerializeField]
        public JSONTileDataModel jsonTileDataModel;

        public TileDataModel Convert()
        {
            TileDataModel tileDataModel = new TileDataModel();
            tileDataModel.TileTypes = new Dictionary<string, int>();
            tileDataModel.TileEdges = new Dictionary<string, int>();
            tileDataModel.TileRules = new Dictionary<string, List<string>>();
            tileDataModel.TileWeights = new Dictionary<string, int>();

            for (int i = 0; i < jsonTileDataModel.TileTypes.Count; i++)
            {
                tileDataModel.TileTypes.Add(jsonTileDataModel.TileTypes[i].Key, jsonTileDataModel.TileTypes[i].Value);
            }

            for (int i = 0; i < jsonTileDataModel.TileEdges.Count; i++)
            {
                tileDataModel.TileEdges.Add(jsonTileDataModel.TileEdges[i].Key, jsonTileDataModel.TileEdges[i].Value);
            }

            for (int i = 0; i < jsonTileDataModel.TileRules.Count; i++)
            {
                tileDataModel.TileRules.Add(jsonTileDataModel.TileRules[i].Key, jsonTileDataModel.TileRules[i].Value);
            }

            for (int i = 0; i < jsonTileDataModel.TileWeights.Count; i++)
            {
                tileDataModel.TileWeights.Add(jsonTileDataModel.TileWeights[i].Key, jsonTileDataModel.TileWeights[i].Value);
            }

            return tileDataModel;
        }
    }

    [Serializable]
    public class JSONTileDataModel
    {
        public List<JSONTileTypeEntry> TileTypes;
        public List<JSONTileEdgeEntry> TileEdges;
        public List<JSONTileRuleEntry> TileRules;
        public List<JSONTileWeightEntry> TileWeights;
    }

    [Serializable]
    public class TileDataModel
    {
        public Dictionary<string, int> TileTypes;
        public Dictionary<string, int> TileEdges;
        public Dictionary<string, List<string>> TileRules;
        public Dictionary<string, int> TileWeights;
    }

    [Serializable]
    public class JSONTileTypeEntry
    {
        public string Key;
        public int Value;
    }

    [Serializable]
    public class JSONTileEdgeEntry
    {
        public string Key;
        public int Value;
    }

    [Serializable]
    public class JSONTileRuleEntry
    {
        public string Key;
        public List<string> Value;
    }

    [Serializable]
    public class JSONTileWeightEntry
    {
        public string Key;
        public int Value;
    }
}
