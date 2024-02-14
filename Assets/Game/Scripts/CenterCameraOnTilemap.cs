using UnityEngine;
using UnityEngine.Tilemaps;

public class CenterCameraOnTilemap : MonoBehaviour
{
    public Tilemap tilemap; // Assign your Tilemap in the Inspector
    public Camera camera; // Assign your Camera in the Inspector, or use Camera.main

    void Start()
    {
        CenterCamera();
    }

    void CenterCamera()
    {
        // Calculate the Tilemap size and center
        BoundsInt bounds = tilemap.cellBounds;
        Vector3 center = bounds.center;

        // Convert the center from cell position to world position
        Vector3 centerWorldPosition = tilemap.CellToWorld(new Vector3Int((int)center.x, (int)center.y, 0)) + new Vector3(tilemap.cellSize.x / 2, tilemap.cellSize.y / 2, 0);

        // If you're using an orthographic camera and want to ensure the whole Tilemap is visible,
        // you may need to adjust the camera's orthographic size here based on the Tilemap's size and the screen aspect ratio.

        // Set the camera position, adjust z as needed to ensure the camera is above the Tilemap
        if (camera != null)
        {
            camera.transform.position = new Vector3(centerWorldPosition.x, centerWorldPosition.y, camera.transform.position.z);
        }
        else
        {
            Debug.LogError("Camera not assigned");
        }
    }
}
