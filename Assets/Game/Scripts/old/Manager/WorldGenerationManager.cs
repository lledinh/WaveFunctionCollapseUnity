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
        private TilemapRuleExtractor TilemapRuleExtractor;
        private WFCTilemapTraverser WFCTilemapTraverser;
        public string ScriptableObjectsOutputPath = "Assets/Game/ScriptableObjects/Generated/";


        public void ExtractRules()
        {
            TilemapRuleExtractor = GetComponent<TilemapRuleExtractor>();
            WFCTilemapTraverser = GetComponent<WFCTilemapTraverser>();
            Utils.CreateFolderIfNotExists(ScriptableObjectsOutputPath);
            Utils.DeleteAllScriptableObjects(ScriptableObjectsOutputPath);
            TilemapRuleExtractor.ExtractRules();
        }

        public void GenerateWorld()
        {
            //WorldGenerator.Generate();
            WFCTilemapTraverser.Generate();
        }
    }
}
