using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlWindow : MonoBehaviour
{
    public Animator controlAnim;

    public Button[] allButtons;
    public Button closeButton; // Close button which is disabled when window is closed

    public Tab[] allTabs; // 0 (tools), 1 (lab), 2 (settings)
    public int currentTab; // Currently open Tab

    public bool windowOpen; // Window is either open or closed, revealing main tab area

    public void OpenTab(int thisTab)
    {
        // When opening a tab, reset to normal time (if fast-forwarding)
        if(Time.timeScale != 1f)
        {
            Time.timeScale = 1f;
        }

        // Clicking the same tab twice closes the window
        if(thisTab == currentTab && windowOpen)
        {
            SetWindowOpen(false);
            return;
        }

        currentTab = thisTab;
        allTabs[currentTab].OpenTab();

        // Play sound
        FindObjectOfType<AudioController>().Play("MenuTap");

        // Display only the selected tab
        for (int i = 0; i < allTabs.Length; i++)
        {
            allTabs[i].tabObject.SetActive(i == thisTab);
        }
    }

    public void CloseAllTabs()
    {
        if (windowOpen)
        {
            SetWindowOpen(false);
        }
    }

    // Reveal the window
    public void SetWindowOpen(bool isOpen)
    {
        windowOpen = isOpen;

        if (isOpen)
        {
            closeButton.enabled = true;

            controlAnim.SetTrigger("OpenWindow");
            controlAnim.ResetTrigger("CloseWindow");

            // Set buttons to opaque
            for (int i = 0; i < allButtons.Length; i++)
            {
                Image buttonImage = allButtons[i].GetComponent<Image>();
                buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 1f);
            }
        }
        else
        {
            closeButton.enabled = false;

            controlAnim.SetTrigger("CloseWindow");
            controlAnim.ResetTrigger("OpenWindow");

            // Set buttons to semi-transparent
            for (int i = 0; i < allButtons.Length; i++)
            {
                Image buttonImage = allButtons[i].GetComponent<Image>();
                buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 0.6f);
            }
        }
    }
}
