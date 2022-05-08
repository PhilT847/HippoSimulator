using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LabTab : Tab
{
    public LabController theLab;

    public Slider nutrientSlider; // Slider for nutrients in water
    public Slider waterOxygenSlider; // Slider for oxygen in water

    public TextMeshProUGUI[] speciesHealths; // Healths for Hippos, flower1s, flower2s, algae, and fish
    public Image[] speciesHealthBackgrounds; // Backgrounds that indicate overall health of each species

    public TextMeshProUGUI[] soilHealths; //Healths 0,1,2,3, and overall

    public override void OpenTab()
    {
        // Play noise
        UpdateLabTab();

        // If not opened yet, open the window
        if (!mainWindow.windowOpen)
        {
            mainWindow.SetWindowOpen(true);
        }
    }

    public override void CloseTab()
    {
        // Play noise
    }

    // Update tab values based on LabController statistics
    public void UpdateLabTab()
    {
        nutrientSlider.value = theLab.nutrientsInWater;
        waterOxygenSlider.value = theLab.oxygenInWater;

        // Hippo, flower1, flower2, algae, fish
        speciesHealths[0].SetText("{0:0}%", theLab.hippoHealth);
        speciesHealths[1].SetText("{0:0}%", theLab.flower1_Health);
        speciesHealths[2].SetText("{0:0}%", theLab.flower2_Health);
        speciesHealths[3].SetText("{0:0}%", theLab.algaeCount);
        speciesHealths[4].SetText("{0:0}%", theLab.fishCount);

        SetBackgroundColor(speciesHealthBackgrounds[0], theLab.hippoHealth);
        SetBackgroundColor(speciesHealthBackgrounds[1], theLab.flower1_Health);
        SetBackgroundColor(speciesHealthBackgrounds[2], theLab.flower2_Health);
        SetBackgroundColor(speciesHealthBackgrounds[3], theLab.algaeCount);
        SetBackgroundColor(speciesHealthBackgrounds[4], theLab.fishCount);

        // Soil healths
        soilHealths[0].SetText("{0:0}%", theLab.soilRejuvenatedPercentage * 100f);
        soilHealths[1].SetText("{0:0}%", theLab.soilHealthyPercentage * 100f);
        soilHealths[2].SetText("{0:0}%", theLab.soilUnhealthyPercentage * 100f);
        soilHealths[3].SetText("{0:0}%", theLab.soilDestroyedPercentage * 100f);
        soilHealths[4].SetText("{0:0}%", theLab.overallSoilHealthPercentage * 100f);
    }

    // Set the color for each background based on abundance
    void SetBackgroundColor(Image background, float value)
    {
        if(value >= 80f) // High
        {
            background.color = new Color32(100, 150, 100, 255);
        }
        else if(value >= 20f) // Normal
        {
            background.color = new Color32(100, 150, 150, 255);
        }
        else // Low
        {
            background.color = new Color32(200, 100, 100, 255);
        }
    }
}
