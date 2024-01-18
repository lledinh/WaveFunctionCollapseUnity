using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TilemapWorldGenerator
{
    public class WorldGenerationManager : MonoBehaviour
    {
        [SerializeField] private TilemapRuleExtractor TilemapRuleExtractor;
        public string ScriptableObjectsOutputPath = "Assets/Game/ScriptableObjects/Generated/";


        public void ExtractRules()
        {
            Utils.CreateFolderIfNotExists(ScriptableObjectsOutputPath);
            Utils.DeleteAllScriptableObjects(ScriptableObjectsOutputPath);
            TilemapRuleExtractor.ExtractRules();
        }
    }
}
