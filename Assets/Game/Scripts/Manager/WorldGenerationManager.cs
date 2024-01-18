using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace TilemapWorldGenerator
{
    public class WorldGenerationManager : MonoBehaviour
    {
        [SerializeField] private TilemapRuleExtractor TilemapRuleExtractor;
        [SerializeField] private WorldGenerator WorldGenerator;
        public string ScriptableObjectsOutputPath = "Assets/Game/ScriptableObjects/Generated/";

        public void ExtractRules()
        {
            Utils.CreateFolderIfNotExists(ScriptableObjectsOutputPath);
            Utils.DeleteAllScriptableObjects(ScriptableObjectsOutputPath);
            TilemapRuleExtractor.ExtractRules();
        }

        public void GenerateWorld()
        {
            WorldGenerator.Generate();
        }
    }
}
