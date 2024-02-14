using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using Newtonsoft.Json;


public class ConfigReader : MonoBehaviour
{
    public string configFilePath = "Assets/Game/Data/TileConfigMine.json";
    public TileDataModel JSONTileDataModel;
    public Config config;

    [ContextMenu("ReadJSON")]
    public TileDataModel ReadJSON()
    {
        string filePath = Path.Combine(Application.dataPath, "Game/Data/TileConfigMine.json");

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            JSONTileDataModel = JsonConvert.DeserializeObject<TileDataModel>(dataAsJson);
            return JSONTileDataModel;
        }
        else
        {
            Debug.LogError("Cannot find JSON file: " + filePath);
        }
        return null;
    }

    [ContextMenu("ShowConfig")]
    public void ShowConfig()
    {
        Debug.Log(config.jsonTileDataModel.TileTypes.Count);
    }
}
