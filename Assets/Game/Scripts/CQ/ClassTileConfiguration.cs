using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.Build.Content;

[Serializable]
public class ClassTileConfiguration
{

    public List<ClassTileType> TileTypes;

    public List<ClassTileEdge> TileEdges;

    public List<ClassTileRule> Rules;

    public Dictionary<ClassTileType, int> TileWeights;

    public ClassTileConfiguration(JSONTileDataModel tileDataModel = null)
    {
        TileTypes = new List<ClassTileType>();
        TileEdges = new List<ClassTileEdge>();
        Rules = new List<ClassTileRule>();
        TileWeights = new Dictionary<ClassTileType, int>();

        /*foreach (KeyValuePair<string, int> kv in tileDataModel.TileTypes)
        {
            TileTypes.Add(new ClassTileType(kv.Key, kv.Value));
        }

        foreach (KeyValuePair<string, int> kv in tileDataModel.TileEdges)
        {
            TileEdges.Add(new ClassTileEdge(kv.Key, kv.Value));
        }

        foreach (KeyValuePair<string, List<string>> kv in tileDataModel.TileRules)
        {
            ClassTileType classTileType = TileTypes.Find(x => x.name == kv.Key);
            List<ClassTileEdge> classTileEdges = new List<ClassTileEdge>();
            foreach (string s in kv.Value)
            {
                ClassTileEdge classTileEdge = TileEdges.Find(x => x.name == s);
                classTileEdges.Add(classTileEdge);
            }

            Rules.Add(new ClassTileRule(classTileType, classTileEdges.ToArray()));
        }

        foreach (KeyValuePair<string, int> kv in tileDataModel.TileWeights)
        {
            ClassTileType classTileType = TileTypes.Find(x => x.name == kv.Key);
            TileWeights.Add(classTileType, kv.Value);
        }*/
    }
}