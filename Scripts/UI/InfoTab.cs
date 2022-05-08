using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoTab : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI infoText;

    public bool tabOpen;

    public void OpenTab(string titleString, string infoString)
    {
        // Remove fast forward
        Time.timeScale = 1f;

        // Set text
        titleText.SetText(titleString);
        infoText.SetText(infoString);

        // If not yet open, animate opening. Otherwise, just set text
        if (!tabOpen)
        {
            GetComponent<Animator>().ResetTrigger("Close");
            GetComponent<Animator>().SetTrigger("Open");
            tabOpen = true;
        }
    }

    public void CloseTab()
    {
        // Remove fast forward
        Time.timeScale = 1f;

        // Play sound
        FindObjectOfType<AudioController>().Play("MenuTap");

        tabOpen = false;
        GetComponent<Animator>().ResetTrigger("Open");
        GetComponent<Animator>().SetTrigger("Close");
    }
}
