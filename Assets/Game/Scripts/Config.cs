using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Config", menuName = "Config")]
public class Config : ScriptableObject
{
    public TileDataModel jsonTileDataModel;
}

[Serializable]
public class TileDataModel
{
    public Dictionary<string, int> TileTypes;
    public Dictionary<string, int> TileEdges;
    public Dictionary<string, List<int>> TileRulesNumber;
    public Dictionary<string, List<string>> TileRules;
    public Dictionary<string, int> TileWeights;

}