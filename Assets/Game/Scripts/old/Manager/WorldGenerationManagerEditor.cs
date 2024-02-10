using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace TilemapWorldGenerator
{
    [CustomEditor(typeof(WorldGenerationManager))]
    public class WorldGenerationManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            WorldGenerationManager script = (WorldGenerationManager) target;

            if (GUILayout.Button("Generated Tile Scriptable Objects"))
            {
                script.ExtractRules();
            }

            if (GUILayout.Button("Generated Map Using Generated SO"))
            {
                script.GenerateWorld();
            }
        }
    }
}
