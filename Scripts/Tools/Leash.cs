using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leash : Tool
{
    public Animator leashAnim;

    public bool leashingHippos;

    public int herdCount;
    public List<Hippo> leashedHippos;

    void Update()
    {
        // Follow cursor
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePosition.x, mousePosition.y, 0f);

        if (Input.GetMouseButton(0))
        {
            if (!leashingHippos)
            {
                LeashHippos();
            }
            else
            {
                MoveLeash();
            }
        }
        else if (leashingHippos)
        {
            ReleaseHippos();
        }
    }

    void LeashHippos()
    {
        // return if clicking on UI elements
        if (IsTouchingWindow())
        {
            return;
        }

        // Play sound
        FindObjectOfType<AudioController>().Play("LeashWhistle");

        leashingHippos = true;
        leashAnim.SetBool("Carrying", true);
        herdCount = 0;
        leashedHippos = new List<Hippo>();

        // Search through all hippos with energy, catching those closest to the Leash
        List<Hippo> allHippos = FindObjectOfType<GameController>().hippoHerd;

        for(int i = 0; i < allHippos.Count; i++)
        {
            if (herdCount < 5 &&
                Vector2.Distance(allHippos[i].transform.position, transform.position) < 3.5f
                && allHippos[i].hippoEnergy > 0f)
            {
                herdCount++;

                leashedHippos.Add(allHippos[i]);

                // Hippo now follows the leash until let go
                allHippos[i].currentBehavior = Hippo.BehaviorType.FOLLOWING_LEASH;
            }
        }
    }

    void MoveLeash()
    {
        Vector3 originPosition = transform.position;

        Vector3 position1 = new Vector3(originPosition.x, originPosition.y, 0f);
        Vector3 position2 = new Vector3(originPosition.x + 1.5f, originPosition.y + 1.5f, 0f);
        Vector3 position3 = new Vector3(originPosition.x + 1.5f, originPosition.y - 1.5f, 0f);
        Vector3 position4 = new Vector3(originPosition.x - 1.5f, originPosition.y - 1.5f, 0f);
        Vector3 position5 = new Vector3(originPosition.x - 1.5f, originPosition.y + 1.5f, 0f);

        Vector3[] allPositions = { position1, position2, position3, position4, position5 };

        for(int i = 0; i < 5; i++)
        {
            if(i < leashedHippos.Count)
            {
                MoveHippoTowards(leashedHippos[i], allPositions[i]);
            }
        }
    }

    void MoveHippoTowards(Hippo thisHippo, Vector3 thisPosition)
    {
        // If the hippo's behavior is changed, release them
        if (thisHippo.currentBehavior != Hippo.BehaviorType.FOLLOWING_LEASH)
        {
            ReleaseHippo(thisHippo);
            return;
        }

        if (Vector3.Distance(thisHippo.transform.position,thisPosition) > 0.01f)
        {
            thisHippo.isMoving = true;

            if (thisHippo.isSwimming)
            {
                thisHippo.hippoAnim.SetBool("Swimming", true);
                thisHippo.hippoAnim.SetBool("Walking", false);
            }
            else
            {
                thisHippo.hippoAnim.SetBool("Swimming", false);
                thisHippo.hippoAnim.SetBool("Walking", true);
            }

            thisHippo.transform.position = Vector3.MoveTowards(thisHippo.transform.position, thisPosition, thisHippo.movementSpeed * 2.5f * Time.deltaTime);

            // Flip hippo if necessary
            if ((thisHippo.transform.position.x < thisPosition.x && !thisHippo.facingRight)
                || (thisHippo.transform.position.x > thisPosition.x && thisHippo.facingRight))
            {
                thisHippo.Flip();
            }

            // Leashed running spends 20 energy per second
            thisHippo.SpendEnergy(20f * Time.deltaTime);

            // Play running particles, if appropriate
            if (!thisHippo.isSwimming && !thisHippo.runningParticles.isPlaying)
            {
                thisHippo.runningParticles.Play();
            }
            else if (thisHippo.isSwimming && thisHippo.runningParticles.isPlaying)
            {
                thisHippo.runningParticles.Stop();
            }
        }
        else
        {
            thisHippo.isMoving = false;

            thisHippo.transform.position = thisPosition;
            thisHippo.hippoAnim.SetBool("Walking", false);

            //End running particles
            thisHippo.runningParticles.Stop();
        }
    }

    void ReleaseHippos()
    {
        leashingHippos = false;
        leashAnim.SetBool("Carrying", false);

        // Remove hippos from leash's control
        for (int i = 0; i < leashedHippos.Count; i++)
        {
            leashedHippos[i].hippoAnim.SetBool("Walking", false);
            leashedHippos[i].currentBehavior = Hippo.BehaviorType.IDLING;
            leashedHippos[i].timeBeforeWalking = Random.Range(2f, 4f);

            //End running particles
            leashedHippos[i].runningParticles.Stop();
        }

        // Clear list
        leashedHippos = new List<Hippo>();
        herdCount = 0;
    }

    // Release a hippo, perhaps due to lost energy
    void ReleaseHippo(Hippo thisHippo)
    {
        thisHippo.hippoAnim.SetBool("Walking", false);
        leashedHippos.Remove(thisHippo);
        herdCount--;
    }
}
