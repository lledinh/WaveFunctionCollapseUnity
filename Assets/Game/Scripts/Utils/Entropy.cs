using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entropy : MonoBehaviour
{
    // ARRAY PASSé PAR COIPIE!!!!!!!
    /*TileOutput GetLowestEntropy(TileOutput[] TilesOutput)
    {
        //Debug.Log($"GetLowestEntropy TilesOutput.Length {TilesOutput.Length}");
        float minEntropy = float.MaxValue;
        int chosenIndex = -1;
        for (int i = 0; i < TilesOutput.Length; i++)
        {
            if (TilesOutput[i].ChosenTile != null && TilesOutput[i].ChosenTile.Name != null && TilesOutput[i].ChosenTile.Name.Length > 0)
            {
                //Debug.Log($"TilesOutput[i].ChosenTile {i} {TilesOutput[i].ChosenTile?.Name}");
                continue;
            }

            float entropy = ShannonEntropy(TilesOutput[i].possibleTiles);
            float noise = UnityEngine.Random.Range(0f, 1f) / 1000f;
            if (entropy - noise < minEntropy)
            {
                minEntropy = entropy - noise;
                chosenIndex = i;
            }
        }
        Debug.Log($"{chosenIndex}");

        return TilesOutput[chosenIndex];
    }

    float ShannonEntropy(List<TileType> possibleTiles)
    {
        float sum_of_weights = 0;
        float sum_of_weight_log_weights = 0;

        foreach (TileType tile in possibleTiles)
        {
            float weight = tile.Weight;
            sum_of_weights += weight;
            sum_of_weight_log_weights += weight * Mathf.Log(weight);
        }
        return Mathf.Log(sum_of_weights) - (sum_of_weight_log_weights / sum_of_weights);
    }*/
}
