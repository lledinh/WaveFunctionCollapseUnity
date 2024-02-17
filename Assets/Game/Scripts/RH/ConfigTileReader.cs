using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RH
{
    public class ConfigTileReader : MonoBehaviour
    {
        public string configFilePath;
        public JSONTileDataModel JSONTileDataModel;
        public ConfigTile config;

        public void Start()
        {
        }

        [ContextMenu("ReadJSON")]
        public JSONTileDataModel ReadJSON()
        {
            Debug.Log("Reading file from: " + configFilePath);
            string filePath = Path.Combine(Application.dataPath, configFilePath);

            if (File.Exists(filePath))
            {
                string dataAsJson = File.ReadAllText(filePath);
                JSONTileDataModel = JsonConvert.DeserializeObject<JSONTileDataModel>(dataAsJson);
                config.jsonTileDataModel = JSONTileDataModel;
                Debug.Log(config.jsonTileDataModel.TileTypes.Count);
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
}
