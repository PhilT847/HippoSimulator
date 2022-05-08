using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergySlider : MonoBehaviour
{
    public Hippo owner;
    public Slider energySlider;

    public Image[] allImages; // all sprites, allowing for transparency editing

    public bool transparent;
    public float timeOpaque = 0f;

    private void Update()
    {
        if (timeOpaque > 0f)
        {
            if (transparent)
            {
                StartCoroutine(SetTransparency(false));
            }

            timeOpaque -= Time.deltaTime;
        }
        else if (!transparent)
        {
            timeOpaque = 0f;

            StartCoroutine(SetTransparency(true));
        }
    }

    public void UpdateSliderValue()
    {
        energySlider.value = owner.hippoEnergy;

        // Set color based on fill value
        if(energySlider.value > 67f)
        {
            energySlider.fillRect.GetComponent<Image>().color = Color.green;
        }
        else if(energySlider.value > 33f)
        {
            energySlider.fillRect.GetComponent<Image>().color = Color.yellow;
        }
        else if (energySlider.value > 0f)
        {
            energySlider.fillRect.GetComponent<Image>().color = Color.red;
        }
        else // Set to "empty" when sleeping
        {
            energySlider.value = 100f;
            energySlider.fillRect.GetComponent<Image>().color = Color.magenta;
        }

        // When spending energy, make the energy slider opaque briefly
        if (timeOpaque < 0.1f)
        {
            timeOpaque = 0.1f;
        }
    }

    // Make the slider invisible when sleeping
    public void SetInvisible()
    {
        // Cancel transparency-altering coroutines
        StopAllCoroutines();

        timeOpaque = 0f;
        transparent = true;

        for (int i = 0; i < allImages.Length; i++)
        {
            Image thisImage = allImages[i];

            thisImage.color = new Color(thisImage.color.r, thisImage.color.g, thisImage.color.b, 0f);
        }
    }

    public IEnumerator SetTransparency(bool setTransparency)
    {
        transparent = setTransparency;

        if (setTransparency) // Transparent
        {
            float transparencyValue = 0.125f;

            while (allImages[0].color.a > transparencyValue)
            {
                for (int i = 0; i < allImages.Length; i++)
                {
                    Image thisImage = allImages[i];

                    // Set over time
                    thisImage.color = new Color(thisImage.color.r, thisImage.color.g, thisImage.color.b, thisImage.color.a - 4f * Time.deltaTime);
                }

                yield return new WaitForEndOfFrame();
            }

            // Ensure that the value reaches max transparency
            for (int i = 0; i < allImages.Length; i++)
            {
                Image thisImage = allImages[i];

                thisImage.color = new Color(thisImage.color.r, thisImage.color.g, thisImage.color.b, transparencyValue);
            }
        }
        else // Opaque
        {
            while (allImages[0].color.a < 1f)
            {
                for (int i = 0; i < allImages.Length; i++)
                {
                    Image thisImage = allImages[i];

                    // Set over time
                    thisImage.color = new Color(thisImage.color.r, thisImage.color.g, thisImage.color.b, thisImage.color.a + 4f * Time.deltaTime);
                }

                yield return new WaitForEndOfFrame();
            }
        }

        yield return null;
    }
}
