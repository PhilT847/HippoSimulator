using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sterilizer : Tool
{
    public int sterilizersAvailable;

    // Syringe sprite changes based on available syringes
    public SpriteRenderer mainSprite;
    public Sprite[] allSyringeSprites;

    // Set sterilizer count based on ToolsTab
    private void Start()
    {
        SetSterilizerCount(FindObjectOfType<ToolsTab>().sterilizerAvailable);
    }

    private void Update()
    {
        // Follow cursor
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePosition.x, mousePosition.y, 0f);

        if (Input.GetMouseButtonDown(0))
        {
            SterilizeHippo();
        }
    }

    void SterilizeHippo()
    {
        // return if clicking on UI elements
        if (IsTouchingWindow() || sterilizersAvailable < 1)
        {
            return;
        }

        // Grab hippos within range
        Collider2D[] detectedHippoColliders = Physics2D.OverlapCircleAll(transform.position, 0.75f);

        for (int i = 0; i < detectedHippoColliders.Length; i++)
        {
            // Sterilize a single non-sterile hippo
            if (detectedHippoColliders[i].GetComponent<Hippo>()
                && !detectedHippoColliders[i].GetComponent<Hippo>().isSterile)
            {
                Hippo sterileHippo = detectedHippoColliders[i].GetComponent<Hippo>();

                sterileHippo.SetSterilizationStatus(true);

                // Reduce syringe count
                FindObjectOfType<ToolsTab>().sterilizerAvailable--;
                FindObjectOfType<ToolsTab>().sterilizerCountText.SetText("{0}", FindObjectOfType<ToolsTab>().sterilizerAvailable);
                SetSterilizerCount(FindObjectOfType<ToolsTab>().sterilizerAvailable);

                // Play sound
                FindObjectOfType<AudioController>().Play("Sterilize");

                // Emit particles
                GetComponentInChildren<ParticleSystem>().Clear();
                GetComponentInChildren<ParticleSystem>().Play();

                // Only sterilize one
                return;
            }
        }
    }

    // Set count and sprite
    public void SetSterilizerCount(int count)
    {
        sterilizersAvailable = count;

        mainSprite.sprite = allSyringeSprites[count];
    }
}
