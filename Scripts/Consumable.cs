using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    public Animator consumableAnim;

    public bool isEdible;

    public float movementSpeed;

    // Hippo attracted to this object
    public Hippo attractedHippo;

    // Object spawned when eaten
    public GameObject objectWhenEaten;

    private void Update()
    {
        if(attractedHippo == null
            && isEdible)
        {
            AttractHippo();
        }
    }

    void AttractHippo()
    {
        // Check hippos within range
        Collider2D[] detectedHippoColliders = Physics2D.OverlapCircleAll(transform.position, 6f);

        for (int i = 0; i < detectedHippoColliders.Length; i++)
        {
            // Attract a nearby non-resting hippo that isn't already chasing down food
            if (detectedHippoColliders[i].GetComponent<Hippo>()
                && detectedHippoColliders[i].GetComponent<Hippo>().hippoEnergy > 0f
                && detectedHippoColliders[i].GetComponent<Hippo>().chasedConsumable == null)
            {
                Hippo newHippo = detectedHippoColliders[i].GetComponent<Hippo>();

                newHippo.chasedConsumable = this;

                newHippo.currentBehavior = Hippo.BehaviorType.FORAGING;

                attractedHippo = newHippo;

                // Only attract one
                return;
            }
        }
    }

    public void EatConsumable()
    {
        // Play sound
        FindObjectOfType<AudioController>().Play("Crunch");

        // Consumables introduce nutrients to the water
        FindObjectOfType<LabController>().AddNutrients(5f);

        var eatenObject = Instantiate(objectWhenEaten, transform.position, Quaternion.identity);
        Destroy(eatenObject, 4f);

        // Remove melon from field, allowing more to be thrown out
        FindObjectOfType<ToolsTab>().totalMelonsOnField--;

        attractedHippo.chasedConsumable = null;
        Destroy(gameObject);
    }

    public IEnumerator ThrowObject(float x_position, float y_position)
    {
        isEdible = false;

        transform.position = new Vector3(x_position, -10f, 0f);

        consumableAnim.SetTrigger("Throw");

        // Roll towards goal area
        Vector3 goalPosition = new Vector3(x_position, y_position, 0f);

        // Roll until reaching goal or hitting water
        while (Vector2.Distance(transform.position, goalPosition) > 0.1f
                && !GetComponent<Collider2D>().IsTouchingLayers(LayerMask.NameToLayer("Water")))
        {
            transform.position = Vector2.MoveTowards(transform.position, goalPosition, movementSpeed * Time.deltaTime);

            // Slow roll as the object moves closer
            if (Vector2.Distance(transform.position, goalPosition) < 4f
                && movementSpeed > 0.5f)
            {
                movementSpeed -= 3f * Time.deltaTime;
            }

            yield return new WaitForEndOfFrame();
        }

        consumableAnim.SetTrigger("Stop");

        isEdible = true;
        yield return null;
    }
}
