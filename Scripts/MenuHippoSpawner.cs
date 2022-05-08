using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHippoSpawner : MonoBehaviour
{
    public GameObject hippoPrefab;

    public float timeBeforeSpawn;

    public Color[] allHippoColors;

    private void Update()
    {
        if(timeBeforeSpawn > 0f)
        {
            timeBeforeSpawn -= Time.deltaTime;
        }
        else
        {
            timeBeforeSpawn = Random.Range(0.5f, 1.5f);

            SpawnHippo();
        }
    }

    void SpawnHippo()
    {
        MenuHippo newHippo = Instantiate(hippoPrefab, new Vector3(-13f, Random.Range(-4f,-5.5f), 0f), Quaternion.identity).GetComponent<MenuHippo>();

        newHippo.menuHippoAnim.SetBool("Walking", true);

        newHippo.currentRunTime = 10f;

        newHippo.runSpeed = Random.Range(3f, 4f);

        // Scale hippo with randomized value
        float chosenSize = Random.Range(-0.2f, 0.2f);

        newHippo.transform.localScale = new Vector3(-0.8f - chosenSize, 0.8f + chosenSize, 1f);

        // Pick two random colors to mix
        Color chosenColor1 = allHippoColors[Random.Range(0, allHippoColors.Length)];
        Color chosenColor2 = allHippoColors[Random.Range(0, allHippoColors.Length)];

        // Blend colors and apply to newHippo
        Color mixedColor = new Color((chosenColor1.r + chosenColor2.r) / 2, (chosenColor1.g + chosenColor2.g) / 2, (chosenColor1.b + chosenColor2.b) / 2, 1f);

        for (int i = 0; i < newHippo.coloredSprites.Length; i++)
        {
            newHippo.coloredSprites[i].color = mixedColor;
        }
    }
}
