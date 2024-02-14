using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapTextOverlay : MonoBehaviour
{
    public Tilemap tilemap;
    public Canvas canvas;
    public GameObject textPrefab; // Assign your Text or TextMeshPro prefab here

    void Start()
    {
        OverlayTextOnTiles();
    }

    void OverlayTextOnTiles()
    {
        for (int n = tilemap.cellBounds.xMin; n < tilemap.cellBounds.xMax; n++)
        {
            for (int p = tilemap.cellBounds.yMin; p < tilemap.cellBounds.yMax; p++)
            {
                Vector3Int localPlace = (new Vector3Int(n, p, (int)tilemap.transform.position.z));
                Vector3 place = tilemap.CellToWorld(localPlace);
                if (tilemap.HasTile(localPlace))
                {
                    // Instantiate the text prefab and position it over the tile
                    GameObject textObject = Instantiate(textPrefab, place, Quaternion.identity, canvas.transform);
                    textObject.transform.position = new Vector3(place.x, place.y, 0); // Center the text over the tile

                    // Set the text value (customize this as needed)
                    if (textObject.GetComponent<TMP_Text>()) // If using TextMeshPro
                    {
                        textObject.GetComponent<TMP_Text>().text = $"{n},{p}\n32"; // Example: sets the text to the tile's coordinates
                    }
                }
            }
        }
    }
}