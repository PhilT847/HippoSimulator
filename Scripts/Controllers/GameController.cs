using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public int currentYear;

    public List<Hippo> hippoHerd;

    public Flower[] potentialFlowers;

    public TextMeshProUGUI yearsText;
    public Slider yearsSlider;

    public UI_Animator uiAnim;

    public GameObject advanceGenerationDisplay;

    public GameObject yearOneDisplay;
    public GameObject yearTenDisplay;
    public GameObject yearTenExtraDisplay;

    // Variables related to muting the game from the Settings menu
    public bool gameMuted;
    public Image muteGameImage;
    public Sprite[] soundImages;

    // Show first-round data
    private void Start()
    {
        InitializeGameStats();

        // Begin ambience
        FindObjectOfType<AudioController>().SetMusic("JungleAmbience");

        // Open year one display which counts as a "window"
        yearOneDisplay.SetActive(true);
        FindObjectOfType<ControlWindow>().windowOpen = true;
    }

    void InitializeGameStats()
    {
        currentYear = 1;

        // Spawn 4 hippos
        for (int i = 0; i < 4; i++)
        {
            GetComponent<HippoCreator>().CreateHippo(new Vector3(Random.Range(-8f, 8f), Random.Range(-4f, 4f), 0f));
        }

        // Nutrients = 12 as there are 4 hippos at the start (3% per hippo)
        FindObjectOfType<LabController>().nutrientsInWater = 12;
        FindObjectOfType<LabController>().lastNutrientsInWater = 12;
        FindObjectOfType<LabController>().oxygenInWater = 80;

        // Initialize fish/algae
        FindObjectOfType<LabController>().fishCount = 90;
        FindObjectOfType<LabController>().algaeCount = 10;

        FindObjectOfType<LabController>().UpdateLabData(true);

        // Refresh sterilizer charges
        FindObjectOfType<ToolsTab>().sterilizerAvailable = 2;

        // Set melons on field to 0
        FindObjectOfType<ToolsTab>().totalMelonsOnField = 0;

        // Time stopped at start
        Time.timeScale = 0f;
    }

    public void BeginSimulation()
    {
        // Begin time and close window
        Time.timeScale = 1f;
        FindObjectOfType<ControlWindow>().windowOpen = false;

        uiAnim.AnimateYearSuccession();

        // Play sound
        FindObjectOfType<AudioController>().Play("NextYear");

        yearOneDisplay.SetActive(false);
    }

    // Advance time, changing water chemistry, flowers, etc.
    public void AdvanceGeneration()
    {
        currentYear++;

        yearsText.SetText("Year {0}", currentYear);
        yearsSlider.value = currentYear;

        // Nutrient baseline is 3 * hippo count
        FindObjectOfType<LabController>().nutrientsInWater = 3f * hippoHerd.Count;

        if (FindObjectOfType<LabController>().nutrientsInWater > 100f)
        {
            FindObjectOfType<LabController>().nutrientsInWater = 100f;
        }

        UpdateFlowerAndSoil();

        FindObjectOfType<LabController>().UpdateLabData(false);

        ResetHippos();

        ClearMap();

        uiAnim.AnimateYearSuccession();

        // Play sound
        FindObjectOfType<AudioController>().Play("NextYear");

        // Refresh sterilizer charges, and set to cursor
        // Note that melon charges are reset in ClearMap()
        FindObjectOfType<ToolsTab>().sterilizerAvailable = 2;
        FindObjectOfType<ToolsTab>().SetTool_Manual(0);

        // Close windows
        FindObjectOfType<ControlWindow>().windowOpen = false;

        // At year 10, activate special events
        if (currentYear == 10)
        {
            StartCoroutine(StartYearTen());
        }
    }

    void UpdateFlowerAndSoil()
    {
        List<Tile> allTiles = FindObjectOfType<TileController>().allTiles;

        for (int i = 0; i < allTiles.Count; i++)
        {
            // Check flowered tiles that haven't already died or had a flower added to them
            if (allTiles[i].currentFlower != null
                && !allTiles[i].alreadyCheckedForFlowers)
            {
                allTiles[i].SpreadFlowers();
            }

            allTiles[i].UpdateSoil();
            allTiles[i].alreadyCheckedForFlowers = false; // allow tile to be checked next round
        }
    }

    public void PutHippoToSleep()
    {
        bool foundAwakeHippo = false;

        for(int i = 0; i < hippoHerd.Count; i++)
        {
            if (!hippoHerd[i].hippoAnim.GetBool("Sleeping"))
            {
                foundAwakeHippo = true;
            }
        }

        // Prompt player to continue if all hippos are asleep
        if (!foundAwakeHippo)
        {
            // Reset timeScale in case it changed
            // Note that the game doesn't run when the graph's open, so it won't interrupt graph reading
            Time.timeScale = 1f;

            if(currentYear < 10)
            {
                // Open advance display window
                advanceGenerationDisplay.SetActive(true);
                FindObjectOfType<ControlWindow>().windowOpen = true;

                // Set tool back to cursor
                FindObjectOfType<ToolsTab>().SetTool_Manual(0);
            }
        }
    }

    void ResetHippos()
    {
        // Breed hippos before they're sterilized
        BreedHippos();

        // Reset hippo energy and remove sleep/sterile status
        for (int i = 0; i < hippoHerd.Count; i++)
        {
            // End coroutines and motion
            hippoHerd[i].StopAllCoroutines();
            hippoHerd[i].hippoAnim.SetBool("Walking", false);
            hippoHerd[i].isMoving = false;

            hippoHerd[i].hippoEnergy = 100f;
            hippoHerd[i].SpendEnergy(0f);

            // Remove sleep and return to idle state
            hippoHerd[i].hippoAnim.SetBool("Sleeping", false);
            hippoHerd[i].currentBehavior = Hippo.BehaviorType.IDLING;
            hippoHerd[i].timeBeforeWalking = Random.Range(2f, 4f);

            // Clear resting spot, if one exists
            if(hippoHerd[i].chosenRestingSpot != null)
            {
                hippoHerd[i].chosenRestingSpot.isOccupied = false;
                hippoHerd[i].chosenRestingSpot = null;
            }

            // Remove sterilization
            hippoHerd[i].isSterile = false;
            hippoHerd[i].sterileCheckSprite.color = Color.clear;

            // Move to a random spot on the field
            hippoHerd[i].transform.position = new Vector3(Random.Range(-15f,15f),Random.Range(-8f,8f),0f);
        }

        // Nutrients in water based on hippo count (3% per hippo)
        // Each hippo adds to nutrient count
        FindObjectOfType<LabController>().nutrientsInWater = 0f;
        FindObjectOfType<LabController>().AddNutrients(3f * hippoHerd.Count);
    }

    void BreedHippos()
    {
        int nonSterileHippos = 0;

        for(int i = 0; i < hippoHerd.Count; i++)
        {
            if (!hippoHerd[i].isSterile)
            {
                nonSterileHippos++;
            }
        }

        int hipposBred = 0;

        // If there are any breeding hippos, breed a certain amount
        if (nonSterileHippos > 0)
        {
            hipposBred = (int)(Random.Range(nonSterileHippos * 0.25f, nonSterileHippos * 0.75f));

            if(hipposBred > 8)
            {
                hipposBred = 8;
            }
        }

        for(int i = 0; i < hipposBred; i++)
        {
            if(hippoHerd.Count < 24)
            {
                FindObjectOfType<HippoCreator>().CreateHippo(new Vector2(Random.Range(-14f, 14f), Random.Range(-8f, 8f)));
            }
        }
    }

    // Toggle faster speed
    public void ToggleFastForward()
    {
        // Play sound
        FindObjectOfType<AudioController>().Play("FastForward");

        if (Time.timeScale != 60f)
        {
            FindObjectOfType<ControlWindow>().CloseAllTabs();

            Time.timeScale = 60f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    // Remove consumables and other added objects from the map
    void ClearMap()
    {
        Consumable[] allConsumables = FindObjectsOfType<Consumable>();

        for(int i = 0; i < allConsumables.Length; i++)
        {
            Destroy(allConsumables[i].gameObject);
        }

        // Clear menu
        if(FindObjectOfType<ControlWindow>().windowOpen)
        {
            FindObjectOfType<ControlWindow>().CloseAllTabs();
        }

        // Return melons on field to 0
        FindObjectOfType<ToolsTab>().totalMelonsOnField = 0;
    }

    // Switch between muted and unmuted
    public void ToggleMute()
    {
        gameMuted = !gameMuted;

        // Play sound
        FindObjectOfType<AudioController>().Play("MenuTap");

        if (gameMuted)
        {
            muteGameImage.sprite = soundImages[1];

            // Mute music
            FindObjectOfType<AudioController>().SetMusic("");
        }
        else
        {
            muteGameImage.sprite = soundImages[0];

            // Play music
            FindObjectOfType<AudioController>().SetMusic("JungleAmbience");
        }
    }

    // Re-enter menu scene
    public void ReturnToMenu()
    {
        // Make timeScale normal, in case it changed for any reason
        Time.timeScale = 1f;

        SceneManager.LoadScene(0);
    }

    // On Year 10, you can play with hippos freely.
    public IEnumerator StartYearTen()
    {
        // Make hippo energy infinite
        for (int i = 0; i < hippoHerd.Count; i++)
        {
            hippoHerd[i].RemoveEnergyRequirement();
        }

        yield return new WaitForSeconds(2.25f);

        yearTenDisplay.SetActive(true);
        yearTenExtraDisplay.SetActive(true);

        // Animate window entrance
        yearTenDisplay.GetComponent<Animator>().SetTrigger("EnterWindow");

        // Reset time
        Time.timeScale = 1f;

        yield return null;
    }
}
