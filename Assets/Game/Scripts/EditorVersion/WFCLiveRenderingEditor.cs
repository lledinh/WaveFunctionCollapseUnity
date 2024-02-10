using TilemapWorldGenerator;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.EditorCoroutines.Editor;
using System.Collections;

namespace WFCEditor
{
    [CustomEditor(typeof(WFCLiveRendering))]
    public class WFCLiveRenderingEditor : Editor
    {
        private float StepDelay = 0.05f;
        EditorCoroutine coroutine;

        public void Clear(WFCLiveRendering script)
        {
            if (coroutine != null)
            {
                script.Clear();
                EditorCoroutineUtility.StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        IEnumerator Generate(WFCLiveRendering script)
        {
            yield return null;
        }

        IEnumerator GenerateStepByStep(WFCLiveRendering script)
        {
            script.GenerateStep();
            bool done = false;
            while (!done)
            {
                done = script.Step();
                yield return new EditorWaitForSeconds(StepDelay);
            }
        }

        IEnumerator GenerateStepByStepLinear(WFCLiveRendering script)
        {
            script.GenerateStepLinear();
            bool done = false;
            while (!done)
            {
                done = script.StepLinear();
                yield return new EditorWaitForSeconds(StepDelay);
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            StepDelay = EditorGUILayout.FloatField("Step Delay", StepDelay);

            WFCLiveRendering script = (WFCLiveRendering) target;

            EditorGUILayout.Space(50);
            GUILayout.Label("---------- NORMAL --------", EditorStyles.boldLabel);

            if (GUILayout.Button("Start Auto Step By Step"))
            {
                Clear(script);
                if (StepDelay <= 0f)
                {
                    coroutine = EditorCoroutineUtility.StartCoroutine(Generate(script), this);
                }
                else
                {
                    coroutine = EditorCoroutineUtility.StartCoroutine(GenerateStepByStep(script), this);
                }
            }

            if (GUILayout.Button("Clear"))
            {
                Clear(script);
            }
            EditorGUILayout.Space(50);
            GUILayout.Label("---------- LINEAR --------", EditorStyles.boldLabel);

            if (GUILayout.Button("Automatic Generation"))
            {
                Clear(script);
                script.GenerateStepLinear();
                bool done = false;
                while (!done)
                {
                    done = script.StepLinear();
                }

            }

            if (GUILayout.Button("Automatic Generation Timelapse"))
            {
                Clear(script);
                coroutine = EditorCoroutineUtility.StartCoroutine(GenerateStepByStepLinear(script), this);
            }


            if (GUILayout.Button("Initalize Step By Step"))
            {
                Clear(script);
                script.GenerateStepLinear();
            }

            if (GUILayout.Button("Next Step"))
            {
                script.StepLinear();
            }

            if (GUILayout.Button("Clear"))
            {
                Clear(script);
            }

            /*EditorGUILayout.Space(50);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Button("World", EditorStyles.miniButtonLeft);
            GUILayout.Button("Local", EditorStyles.miniButtonRight);
            EditorGUILayout.EndHorizontal();*/
        }
    }
}
