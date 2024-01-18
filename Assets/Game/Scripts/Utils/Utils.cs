using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using System.IO;
using UnityEngine;
using TilemapWorldGenerator;

public class Utils
{
    public static void CreateFolderIfNotExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            AssetDatabase.Refresh();
            Debug.Log($"Created folder: {path}");
        }
    }

    public static void DeleteAllScriptableObjects(string directoryPath)
    { // Set your directory path here

        // Ensure the directory exists
        if (!Directory.Exists(directoryPath))
        {
            Debug.LogError("Directory does not exist: " + directoryPath);
            return;
        }

        // Get all asset paths in the directory
        string[] assetPaths = AssetDatabase.GetAllAssetPaths();
        foreach (string assetPath in assetPaths)
        {
            // Check if the asset is in the specified directory and is a ScriptableObject
            if (assetPath.StartsWith(directoryPath) && AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath) != null)
            {
                // Delete the asset
                AssetDatabase.DeleteAsset(assetPath);
            }
        }

        // Refresh the AssetDatabase to reflect changes
        AssetDatabase.Refresh();
    }

    public static Node[] GetAllNodesDefinitionFromDirectory(string directoryPath)
    {
        // Ensure the directory exists
        if (!Directory.Exists(directoryPath))
        {
            Debug.LogError("Directory does not exist: " + directoryPath);
            return null;
        }

        List<Node> nodes = new List<Node>();

        // Get all asset paths in the directory
        string[] assetPaths = AssetDatabase.GetAllAssetPaths();
        foreach (string assetPath in assetPaths)
        {
            // Check if the asset is in the specified directory and is a Node
            Node node = AssetDatabase.LoadAssetAtPath<Node>(assetPath);
            if (node != null && assetPath.StartsWith(directoryPath))
            {
                nodes.Add(node);
            }
        }

        // Convert the list to an array if needed
        Node[] nodeArray = nodes.ToArray();

        return nodeArray;
    }
}
