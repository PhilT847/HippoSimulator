using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HippoCreator : MonoBehaviour
{
    public GameObject hippoPrefab;

    // Hippos are a mix of two colors from this palette.
    public Color[] allHippoColors;

    public float hippoSizeVariance; // Size variance (ex. 0.1 means hippos are sized from 0.9 to 1.1)

    public void CreateHippo(Vector3 spawnPos)
    {
        Hippo newHippo = Instantiate(hippoPrefab, spawnPos, Quaternion.identity).GetComponent<Hippo>();

        // Scale hippo with randomized value
        float chosenSize = Random.Range(-hippoSizeVariance, hippoSizeVariance);

        newHippo.transform.localScale = new Vector3(0.8f + chosenSize, 0.8f + chosenSize, 1f);

        // Pick two random colors to mix
        Color chosenColor1 = allHippoColors[Random.Range(0, allHippoColors.Length)];
        Color chosenColor2 = allHippoColors[Random.Range(0, allHippoColors.Length)];

        // Blend colors and apply to newHippo
        Color mixedColor = new Color((chosenColor1.r + chosenColor2.r) / 2, (chosenColor1.g + chosenColor2.g) / 2, (chosenColor1.b + chosenColor2.b) / 2, 1f);

        for (int i = 0; i < newHippo.coloredBodyParts.Length; i++)
        {
            newHippo.coloredBodyParts[i].color = mixedColor;
        }

        // Spawn as a hippo walking on land
        newHippo.currentBehavior = Hippo.BehaviorType.IDLING;
        newHippo.timeBeforeWalking = Random.Range(2f, 5f);

        newHippo.hippoEnergy = 100f;

        newHippo.energySlider.SetTransparency(true);

        // Add to list
        FindObjectOfType<GameController>().hippoHerd.Add(newHippo);
    }
}
