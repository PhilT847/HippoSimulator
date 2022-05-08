using UnityEngine;

public class NullTool : Tool
{
    public SpriteRenderer gloveSprite;
    public Sprite[] allSprites;

    public ParticleSystem tapParticlesGrass;
    public ParticleSystem tapParticlesWater;

    private void Update()
    {
        // Follow cursor
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePosition.x, mousePosition.y, 0f);

        if (Input.GetMouseButtonDown(0))
        {
            TapCursor();
        }

        // Click to change sprite
        if (Input.GetMouseButton(0))
        {
            gloveSprite.sprite = allSprites[1];
        }
        else
        {
            gloveSprite.sprite = allSprites[0];
        }
    }

    void TapCursor()
    {
        // return if clicking on UI elements
        if (IsTouchingWindow())
        {
            return;
        }

        bool touchingWater = false;
        Hippo tappedHippo = null;

        // Check if the player clicked a water tile before continuing
        Collider2D[] detectedColliders = Physics2D.OverlapCircleAll(transform.position, 0.125f);

        for (int i = 0; i < detectedColliders.Length; i++)
        {
            // Hitting water changes the particle system and sound played
            if (detectedColliders[i].GetComponent<Tile>()
                && !detectedColliders[i].GetComponent<Tile>().isLand)
            {
                touchingWater = true;
            }
        }

        // Check if the player clicked a hippo
        Collider2D[] detectedHippos = Physics2D.OverlapCircleAll(new Vector3(transform.position.x, transform.position.y - 0.5f, 0f), 0.34f);

        for (int i = 0; i < detectedHippos.Length; i++)
        {
            // Tapping a hippo plays a different noise
            if (detectedHippos[i].GetComponent<Hippo>())
            {
                tappedHippo = detectedHippos[i].GetComponent<Hippo>();
            }
        }

        // If no windows are open and not fast-forwarding, play particles/sound
        if (!FindObjectOfType<ControlWindow>().windowOpen)
        {
            if(tappedHippo == null || tappedHippo.isMoving) // Tap tile
            {
                if (touchingWater)
                {
                    //tapParticlesWater.Clear();
                    tapParticlesWater.Play();

                    // Play sound
                    FindObjectOfType<AudioController>().Play("TapWater");
                }
                else
                {
                    //tapParticlesGrass.Clear();
                    tapParticlesGrass.Play();

                    // Play sound
                    FindObjectOfType<AudioController>().Play("TapGround");
                }
            }
            else // Tap hippo, so long as it's standing still
            {
                
                // Play random duck sound
                int randomizer = Random.Range(0, 2);

                // Duck sound pitch based on hippo size
                // localScale.x varies from 0.6 -> 1 (1.2 -> 2 multiplied by 2)
                // Vary sound from 1.4 -> 0.6
                float soundPitch = 2.6f - (tappedHippo.transform.localScale.x * 2f);

                Debug.Log("Sound Pitch: " + soundPitch);

                if (randomizer == 0)
                {
                    FindObjectOfType<AudioController>().PlayWithPitch("Ducky1", soundPitch);
                }
                else
                {
                    FindObjectOfType<AudioController>().PlayWithPitch("Ducky2", soundPitch);
                }

                // Animate
                tappedHippo.hippoAnim.SetTrigger("Tapped");

                // Keep hippo from running briefly
                if(tappedHippo.timeBeforeWalking < 0.5f)
                {
                    tappedHippo.timeBeforeWalking = 0.5f;
                }
            }
        }
    }
}
