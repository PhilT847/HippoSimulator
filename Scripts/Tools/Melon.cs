using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melon : Tool
{
    public GameObject watermelon;

    public SpriteRenderer mainSprite;
    public Sprite[] melonSprites; // 0 (can throw), 1 (can't throw)

    private void Update()
    {
        // Follow cursor
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePosition.x, mousePosition.y, 0f);

        if (Input.GetMouseButtonDown(0))
        {
            ThrowMelon();
        }

        bool showBadSprite = false;

        // Ensure that the player did not click a water tile before continuing
        Collider2D[] detectedWaterColliders = Physics2D.OverlapCircleAll(transform.position, 2f);

        for (int i = 0; i < detectedWaterColliders.Length; i++)
        {
            // Return when clicking a water tile
            if (detectedWaterColliders[i].GetComponent<Tile>()
                && !detectedWaterColliders[i].GetComponent<Tile>().isLand)
            {
                showBadSprite = true;
            }
        }

        // Disable tool if touching a window or if the total melons on the field >= 10
        // Also disable if timeScale is messed up
        if (showBadSprite
            || FindObjectOfType<ToolsTab>().totalMelonsOnField > 9 
            || Time.timeScale != 1f)
        {
            mainSprite.sprite = melonSprites[1];
        }
        else
        {
            mainSprite.sprite = melonSprites[0];
        }
    }

    void ThrowMelon()
    {
        // return if clicking on UI elements, or if total melons are >= 10, or if time is fast-forwarded
        if (IsTouchingWindow() 
            || FindObjectOfType<ToolsTab>().totalMelonsOnField > 9
            || Time.timeScale != 1f)
        {
            return;
        }

        // Ensure that the player did not click a water tile before continuing
        Collider2D[] detectedWaterColliders = Physics2D.OverlapCircleAll(transform.position, 2f);

        for (int i = 0; i < detectedWaterColliders.Length; i++)
        {
            // Return when clicking a water tile
            if (detectedWaterColliders[i].GetComponent<Tile>()
                && !detectedWaterColliders[i].GetComponent<Tile>().isLand)
            {
                return;
            }
        }

        // Play sound
        FindObjectOfType<AudioController>().Play("ThrowMelon");

        // Throw melon to mouse position
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Set melon values to reasonable values if out of bounds
        if(mousePosition.y < -8f)
        {
            mousePosition.y = -8f;
        }
        else if (mousePosition.y > 8f)
        {
            mousePosition.y = 8f;
        }

        if (mousePosition.x < -14f)
        {
            mousePosition.x = -14f;
        }
        else if (mousePosition.x > 14f)
        {
            mousePosition.x = 14f;
        }

        // Add a melon to the field
        FindObjectOfType<ToolsTab>().totalMelonsOnField++;

        Consumable newWatermelon = Instantiate(watermelon, new Vector3(Random.Range(-10f, 10f), -10f, 0f), Quaternion.identity).GetComponent<Consumable>();

        newWatermelon.StartCoroutine(newWatermelon.ThrowObject(mousePosition.x, mousePosition.y));
    }
}
