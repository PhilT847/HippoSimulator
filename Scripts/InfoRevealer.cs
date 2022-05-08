using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoRevealer : MonoBehaviour
{
    public string infoTitle;
    public string infoContent;

    public bool infoActive;

    // Begin game by hiding the revealer
    private void Start()
    {
        HideRevealer();
    }

    // When a Tool presses right click on an open Revealer, show info on InfoTab
    public void OpenInfo()
    {
        // Play sound
        FindObjectOfType<AudioController>().Play("InfoClick");

        FindObjectOfType<InfoTab>().OpenTab(infoTitle, infoContent);
    }

    public void ShowRevealer()
    {
        if (!infoActive)
        {
            infoActive = true;

            if (GetComponent<Image>())
            {
                GetComponent<Image>().color = Color.white;
            }
            else
            {
                GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }

    public void HideRevealer()
    {
        infoActive = false;

        if (GetComponent<Image>())
        {
            GetComponent<Image>().color = Color.clear;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.clear;
        }
    }
}
