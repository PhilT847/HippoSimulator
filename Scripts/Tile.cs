using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    // Conditions of the tile; does it have a flower? is it trampled?
    public Flower currentFlower;
    public bool isTrampled;
    public bool isLand;
    public bool alreadyCheckedForFlowers; // When updating, ignore tiles that have already been run through

    // For land tiles. 0 (healthy), 1 (damaged), 2 (broken)
    public int soilQuality;

    public Sprite[] allBaseSprites; // Sprites for water, grass, and soil

    public SpriteRenderer tileSprite; // sprite for the main tile
    public SpriteRenderer flowerSprite; // sprite when flowers, objects, etc. are on this tile
    public SpriteRenderer trampleSprite; // sprite when this tile is trampled

    // Particles when dust is kicked up
    public ParticleSystem dustParticles;

    // Used to determine positioning, flower growth, etc...
    public int position_x;
    public int position_y;

    public void AddFlower(Flower thisFlower)
    {
        currentFlower = Instantiate(thisFlower.gameObject, transform.position, Quaternion.identity, transform).GetComponent<Flower>();

        flowerSprite.sprite = currentFlower.appearanceSprite;
    }

    // Return all neighboring Tiles
    public List<Tile> GetNeighboringTiles()
    {
        List<Tile> neighbors = new List<Tile>();

        // Right
        if(FindObjectOfType<TileController>().GetTileAt(position_x + 1, position_y) != null)
        {
            neighbors.Add(FindObjectOfType<TileController>().GetTileAt(position_x + 1, position_y));
        }

        // Left
        if (FindObjectOfType<TileController>().GetTileAt(position_x - 1, position_y) != null)
        {
            neighbors.Add(FindObjectOfType<TileController>().GetTileAt(position_x - 1, position_y));
        }

        // Up
        if (FindObjectOfType<TileController>().GetTileAt(position_x, position_y + 1) != null)
        {
            neighbors.Add(FindObjectOfType<TileController>().GetTileAt(position_x, position_y + 1));
        }

        // Down
        if (FindObjectOfType<TileController>().GetTileAt(position_x, position_y - 1) != null)
        {
            neighbors.Add(FindObjectOfType<TileController>().GetTileAt(position_x, position_y - 1));
        }

        return neighbors;
    }

    void TrampleTile(bool isNowTrampled)
    {
        // Trample non-trampled tiles, or reset trampled status
        if (isNowTrampled)
        {
            if (!isTrampled)
            {
                isTrampled = true;
                dustParticles.Play();
                trampleSprite.color = Color.white;

                // Play sound
                FindObjectOfType<AudioController>().Play("Footstep");
            }
        }
        else
        {
            isTrampled = false;
            trampleSprite.color = Color.clear;
        }
    }

    // Interaction when a hippo steps on a tile
    public void StepOnTile(Hippo touchingHippo)
    {
        if (isLand && !isTrampled)
        {
            float diceRoll = Random.Range(0f, 1f);

            // 1 in 4 chance for a moving hippo to trample soil
            if(diceRoll < 0.25f)
            {
                TrampleTile(true);
            }
        }
        else if (!isLand 
                && !touchingHippo.isSwimming
                && !touchingHippo.GetComponent<Collider2D>().IsTouchingLayers(LayerMask.NameToLayer("Ground"))) // Enter water if not touching ground
        {
            touchingHippo.SetSwimming(true);
        }
    }

    // Turn a tile into a water tile
    public void SetToWaterTile(bool isNowWater)
    {
        isLand = !isNowWater;

        if (isNowWater)
        {
            tileSprite.sprite = allBaseSprites[4];
            gameObject.layer = LayerMask.NameToLayer("Water");
        }
        else
        {
            tileSprite.sprite = allBaseSprites[soilQuality];
            gameObject.layer = LayerMask.NameToLayer("Ground");
        }
    }

    // Attempt to spread flowers to neighboring tiles
    public void SpreadFlowers()
    {
        List<Tile> neighbors = GetNeighboringTiles();

        bool flowerDead = false;
        float deathRoll = Random.Range(0f, 1f);

        // Potentially kill a trampled flower before spreading
        if (isTrampled && deathRoll < currentFlower.chanceOfTrampleDeath)
        {
            flowerDead = true;
        }

        if (!flowerDead)
        {
            for (int i = 0; i < neighbors.Count; i++)
            {
                float diceRoll = Random.Range(0f, 1f);

                float rollNeededToSpread = currentFlower.chanceOfSpreading;

                // Add trample mod to chance, if applicable
                // Trampled neighbors are easier/harder to grow into based on the flower
                if (neighbors[i].isTrampled)
                {
                    rollNeededToSpread += currentFlower.trampleSpreadMod;
                }

                // Spread flowers if the roll is right, and you can beat out the other flower
                if (diceRoll < rollNeededToSpread && !neighbors[i].alreadyCheckedForFlowers)
                {
                    // Must be a flowerless tile, or one with a weaker flower on it
                    if (neighbors[i].currentFlower == null
                        || (currentFlower.competitivePower > neighbors[i].currentFlower.competitivePower))
                    {
                        neighbors[i].AddFlower(currentFlower);
                        neighbors[i].alreadyCheckedForFlowers = true; // "Check" this tile for the rest of the update

                        neighbors[i].GetComponent<Animator>().SetTrigger("GrowFlower"); // Animate growth
                    }
                }
            }
        }
        else // Kill the flower
        {
            currentFlower.KillFlower(this);
        }
    }

    public void UpdateSoil()
    {
        if (isTrampled)
        {
            // Trampling has the following effect: 0->2, 1->0, 2->3
            if(soilQuality == 0)
            {
                SetSoilQuality(2);
            }
            else if(soilQuality == 1)
            {
                SetSoilQuality(0);
            }
            else if(soilQuality == 2)
            {
                SetSoilQuality(3);
            }
        }
        else if (soilQuality == 3 || soilQuality == 2) // Soils 2 and 3 can increase in quality when ignored
        {
            SetSoilQuality(soilQuality - 1);
        }

        // Remove trampled status from each tile
        TrampleTile(false);
    }

    // Change appearance and quality based on status
    void SetSoilQuality(int thisSoil)
    {
        soilQuality = thisSoil;

        tileSprite.sprite = allBaseSprites[thisSoil];
    }
}
