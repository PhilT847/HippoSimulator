using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hippo : MonoBehaviour
{
    public SpriteRenderer[] coloredBodyParts; // Parts colored by HippoCreator.CreateHippo()
    public SpriteRenderer[] allBodyParts; // All body parts, sorted in sort order
    public SpriteRenderer shadowSprite;
    
    public Animator hippoAnim; // Hippo's animator

    // Variables for hippo behavior
    public enum BehaviorType { IDLING, SLEEPING, FORAGING, TRYING_TO_REST, FOLLOWING_LEASH };
    public BehaviorType currentBehavior;

    // Behavior for IDLING
    public float movementSpeed;
    public float timeBeforeWalking = 0f;
    public bool isSwimming;
    public bool facingRight = false;
    public bool isMoving;
    public Transform flippedBody; // the body part that's flipped
    public ParticleSystem runningParticles; // particles when running

    // Breeding ability
    public bool isSterile;
    public SpriteRenderer sterileCheckSprite;

    // Energy (100 per day, reduced by activities. Not needed by year 10)
    public float hippoEnergy;
    public EnergySlider energySlider;
    public bool infiniteEnergy;

    // Rest-related variables
    public RestingSpot chosenRestingSpot;

    // Foraging-related variables
    public Consumable chasedConsumable;
    public int melonsEaten;

    // Hippo energy bar
    public GameObject hippoEnergyVisual;

    private void Update()
    {
        UpdateSpriteRenderOrder();

        UpdateBehavior();
    }

    // Change sprite sort order based on Y-position to prevent meshing between hippo bodies
    void UpdateSpriteRenderOrder()
    {
        int verticalOrder = (int) (transform.position.y * -10000f);

        // Update shadow, back ear, torso, bandage, head, front ear, face in that order 
        allBodyParts[0].sortingOrder = verticalOrder + 0;
        allBodyParts[1].sortingOrder = verticalOrder + 4;
        allBodyParts[2].sortingOrder = verticalOrder + 8;
        allBodyParts[6].sortingOrder = verticalOrder + 10;
        allBodyParts[3].sortingOrder = verticalOrder + 12;
        allBodyParts[4].sortingOrder = verticalOrder + 16;
        allBodyParts[5].sortingOrder = verticalOrder + 20;
    }

    void UpdateBehavior()
    {
        switch (currentBehavior)
        {
            case BehaviorType.IDLING:
                Idle();
                break;
            case BehaviorType.FORAGING:
                Forage();
                break;
            case BehaviorType.FOLLOWING_LEASH:
                Follow_Leash();
                break;
            case BehaviorType.TRYING_TO_REST:
                Look_For_Water();
                break;
        }
    }

    void Idle()
    {
        if(timeBeforeWalking > 0f)
        {
            timeBeforeWalking -= Time.deltaTime;
        }
        else
        {
            // Set walk time so that the hippo doesn't walk again instantly
            timeBeforeWalking = 99f;
            StartCoroutine(WalkToNewSpot());
        }
    }

    // Chase down a consumable and eat it when close enough
    void Forage()
    {
        Vector3 foodPosition = chasedConsumable.transform.position;

        // Go to position; otherwise, sleep
        if (Vector2.Distance(transform.position, foodPosition) > 0.1f)
        {
            isMoving = true;

            transform.position = Vector2.MoveTowards(transform.position, foodPosition, movementSpeed * 1.5f * Time.deltaTime);

            if (facingRight && foodPosition.x < transform.position.x
                || !facingRight && foodPosition.x > transform.position.x)
            {
                Flip();
            }

            hippoAnim.SetBool("Walking", !isSwimming);

            // Play running particles, if appropriate
            if (!isSwimming && !runningParticles.isPlaying)
            {
                runningParticles.Play();
            }
            else if (isSwimming && runningParticles.isPlaying)
            {
                runningParticles.Stop();
            }
        }
        else // once close enough, eat the consumable
        {
            isMoving = false;

            hippoAnim.SetBool("Walking", false);

            // End running particles
            runningParticles.Stop();

            // Add to total melons eaten
            melonsEaten++;

            chasedConsumable.EatConsumable();

            currentBehavior = BehaviorType.IDLING; // return to idle status

            // Foraging spends lots of energy
            SpendEnergy(50.1f);

            // Reset walking time
            timeBeforeWalking = Random.Range(4f, 8f);

            return;
        }
    }

    void Follow_Leash()
    {

    }

    void Look_For_Water()
    {
        Vector3 searchPosition = chosenRestingSpot.transform.position;

        // Go to position; otherwise, sleep
        if (Vector2.Distance(transform.position, searchPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, searchPosition, movementSpeed * 1.5f * Time.deltaTime);
            isMoving = true;

            if (facingRight && searchPosition.x < transform.position.x
                || !facingRight && searchPosition.x > transform.position.x)
            {
                Flip();
            }

            hippoAnim.SetBool("Walking", !isSwimming);

            // Play running particles, if appropriate
            if (!isSwimming && !runningParticles.isPlaying)
            {
                runningParticles.Play();
            }
            else if (isSwimming && runningParticles.isPlaying)
            {
                runningParticles.Stop();
            }
        }
        else if (!hippoAnim.GetBool("Sleeping")) // Fall asleep
        {
            isMoving = false;

            hippoAnim.SetBool("Sleeping", true);

            // End running particles
            runningParticles.Stop();

            // Random chance to flip
            float randomizeFlip = Random.Range(0f, 1f);

            if(randomizeFlip > 0.5f)
            {
                Flip();
            }

            // Make the slider invisible
            energySlider.SetInvisible();

            // Notify GameController that this hippo is now asleep
            FindObjectOfType<GameController>().PutHippoToSleep();
        }
    }

    public IEnumerator WalkToNewSpot()
    {
        // Find a new position a random distance away
        Vector3 newPosition = FindNewPosition();

        if(facingRight && newPosition.x < transform.position.x
            || !facingRight && newPosition.x > transform.position.x)
        {
            Flip();
        }

        if(Vector2.Distance(transform.position, newPosition) > 0.1f)
        {
            isMoving = true;

            // If not swimming, animate running
            hippoAnim.SetBool("Walking", !isSwimming);

            // Play running particles, if appropriate
            if (!isSwimming && !runningParticles.isPlaying)
            {
                runningParticles.Play();
            }
            else if (isSwimming && runningParticles.isPlaying)
            {
                runningParticles.Stop();
            }
        }
        else // When the distance is too low, cancel this command and rest longer
        {
            // Reset walking time
            timeBeforeWalking = Random.Range(4f, 8f);
            isMoving = false;

            // End running particles
            runningParticles.Stop();

            yield break;
        }

        while(Vector2.Distance(transform.position, newPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, newPosition, movementSpeed * Time.deltaTime);

            hippoAnim.SetBool("Walking", !isSwimming);

            // Play running particles, if appropriate
            if (!isSwimming && !runningParticles.isPlaying)
            {
                runningParticles.Play();
            }
            else if (isSwimming && runningParticles.isPlaying)
            {
                runningParticles.Stop();
            }

            // Spend 5 energy per second while moving to a new spot
            SpendEnergy(5f * Time.deltaTime);

            // If behavior state changes for any reason, cancel
            if(currentBehavior != BehaviorType.IDLING)
            {
                isMoving = false;

                // End running particles
                runningParticles.Stop();

                yield break;
            }

            yield return new WaitForEndOfFrame();
        }

        hippoAnim.SetBool("Walking", false);
        isMoving = false;

        // End running particles
        runningParticles.Stop();

        // Reset walking time
        timeBeforeWalking = Random.Range(4f, 8f);

        yield return null;
    }

    // FindNewPosition picks a new Vector3 position for an idling hippo
    Vector3 FindNewPosition()
    {
        Vector3 newPosition = Vector3.zero;

        // If the hippo's x/y positions are near boundaries, force them closer to the middle
        float xPosAdjust = 0f;
        float yPosAdjust = 0f;

        if (transform.position.x > 14f)
        {
            xPosAdjust = -5f;
        }
        else if (transform.position.x < -14f)
        {
            xPosAdjust = 5f;
        }

        if (transform.position.y > 7f)
        {
            yPosAdjust = -3f;
        }
        else if (transform.position.y < -7f)
        {
            yPosAdjust = 3f;
        }

        float new_x = Random.Range(transform.position.x - 5f + xPosAdjust, transform.position.x + 5f + xPosAdjust);
        float new_y = Random.Range(transform.position.y - 3f + yPosAdjust, transform.position.y + 3f + yPosAdjust);

        // Keep hippo within boundaries
        if (new_x > 15f)
        {
            new_x = Random.Range(14f, 15f);
        }
        else if (new_x < -15f)
        {
            new_x = Random.Range(-15f, -14f);
        }

        if (new_y > 8f)
        {
            new_y = Random.Range(7f, 8f);
        }
        else if (new_y < -8f)
        {
            new_y = Random.Range(-8f, -7f);
        }

        newPosition.x = new_x;
        newPosition.y = new_y;

        return newPosition;
    }

    public void Flip()
    {
        facingRight = !facingRight;

        flippedBody.transform.localScale = new Vector3(flippedBody.transform.localScale.x * -1f, flippedBody.transform.localScale.y, 1f);
    }

    // Set sterilization status
    public void SetSterilizationStatus(bool sterilized)
    {
        // Sterilize a fertile hippo
        if (!isSterile && sterilized)
        {
            isSterile = true;
        }
        else if (isSterile && !sterilized) // De-sterilize
        {
            isSterile = false;
        }
        
        // Animate and set bandage color based on sterilization status
        if (isSterile)
        {
            hippoAnim.SetTrigger("Tapped");

            sterileCheckSprite.color = Color.white;
        }
        else
        {
            sterileCheckSprite.color = Color.clear;
        }
    }

    public void SetSwimming(bool swimming)
    {
        isSwimming = swimming;

        hippoAnim.SetBool("Swimming", swimming);

        // When swimming, stop the walking animation
        if (swimming)
        {
            hippoAnim.SetBool("Walking", false);
            shadowSprite.color = Color.clear;
        }
        else
        {
            shadowSprite.color = Color.white;
        }
    }

    // Spend the hippo's energy
    public void SpendEnergy(float value)
    {
        // If the hippo has infinite energy, return
        if (infiniteEnergy)
        {
            return;
        }

        hippoEnergy -= value;

        // At 0 energy, try to sleep
        if(hippoEnergy <= 0f)
        {
            hippoEnergy = 0f;

            FindRestingSpot();
            currentBehavior = BehaviorType.TRYING_TO_REST;
        }

        energySlider.UpdateSliderValue();
    }

    // Search through every available resting spot and choose an open space
    void FindRestingSpot()
    {
        RestingSpot[] allRestingSpots = FindObjectsOfType<RestingSpot>();

        for(int i = 0; i < allRestingSpots.Length; i++)
        {
            // Choose the empty spot
            if(!allRestingSpots[i].isOccupied)
            {
                allRestingSpots[i].isOccupied = true;
                chosenRestingSpot = allRestingSpots[i];
                return;
            }
        }
    }

    // At year 10, hippos don't need energy; you can move them freely
    public void RemoveEnergyRequirement()
    {
        infiniteEnergy = true;
        hippoEnergyVisual.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Step on tiles
        // Note that when a hippo has infinite energy (year 10), they don't trample tiles anymore
        if (collision.GetComponent<Tile>() && isMoving && !infiniteEnergy)
        {
            collision.GetComponent<Tile>().StepOnTile(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Exit water tile; if the hippo is no longer touching water, stop swimming.
        if (collision.GetComponent<Tile>())
        {
            bool touchingAnyWater = false;

            // Grab hippos within range
            Collider2D[] detectedTiles = Physics2D.OverlapCircleAll(transform.position, 0.1f);

            for (int i = 0; i < detectedTiles.Length; i++)
            {
                // Sterilize a single non-sterile hippo
                if (detectedTiles[i].GetComponent<Tile>()
                    && !detectedTiles[i].GetComponent<Tile>().isLand)
                {
                    touchingAnyWater = true;
                }
            }

            // If touching any water, then keep swimming
            SetSwimming(touchingAnyWater);
        }
    }
}
