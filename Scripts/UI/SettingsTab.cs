using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsTab : Tab
{
    public bool graphOpen;
    public GameObject graphObject;

    public GameObject promptExitWindow;

    public GameObject referencesWindow;
    public override void OpenTab()
    {
        // Play noise

        // If not opened yet, open the window
        if (!mainWindow.windowOpen)
        {
            mainWindow.SetWindowOpen(true);
        }
    }

    public override void CloseTab()
    {
        graphOpen = false;
        graphObject.SetActive(false);

        // Play noise
    }

    public void ToggleGraph()
    {
        // Play sound
        FindObjectOfType<AudioController>().Play("MenuTap");

        graphOpen = !graphOpen;
        graphObject.SetActive(graphOpen);

        // Freeze time while viewing graph
        if (graphOpen)
        {
            Time.timeScale = 0f;

            // Change to pointer tool
            FindObjectOfType<ToolsTab>().SetTool_Manual(0);

            // Deselect graph points beyond the first three (hippo/blue/red pops), for simplicity
            graphObject.GetComponent<Graph>().DisplayRelevantLines();

        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void PromptExit()
    {
        // Play sound
        FindObjectOfType<AudioController>().Play("MenuTap");

        // Freeze time
        Time.timeScale = 0f;

        // Change to pointer tool when trying to exit
        FindObjectOfType<ToolsTab>().SetTool_Manual(0);

        promptExitWindow.SetActive(true);
    }

    public void CancelExit()
    {
        // Play sound
        FindObjectOfType<AudioController>().Play("MenuTap");

        // Return time
        Time.timeScale = 1f;

        promptExitWindow.SetActive(false);
    }

    public void OpenReferences()
    {
        // Play sound
        FindObjectOfType<AudioController>().Play("MenuTap");

        // Freeze time
        Time.timeScale = 0f;

        // Change to pointer tool when reading references
        FindObjectOfType<ToolsTab>().SetTool_Manual(0);

        referencesWindow.SetActive(true);
    }

    public void CloseReferences()
    {
        // Play sound
        FindObjectOfType<AudioController>().Play("MenuTap");

        // Return time
        Time.timeScale = 1f;

        referencesWindow.SetActive(false);
    }
}
