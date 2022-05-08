using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToolsTab : Tab
{
    public Tool[] allTools;

    // Variables related to tool quantities
    public int sterilizerAvailable;
    public int totalMelonsOnField;

    // Sterilizer count display
    public TextMeshProUGUI sterilizerCountText;

    public override void OpenTab()
    {
        // Play noise

        // If not opened yet, open the window
        if (!mainWindow.windowOpen)
        {
            mainWindow.SetWindowOpen(true);
        }

        // Set count text on sterilizer tool to current sterilizer count
        sterilizerCountText.SetText("{0}", sterilizerAvailable);
    }

    public override void CloseTab()
    {
        // Play noise
    }

    public void SetTool(int index)
    {
        PlayerCursor cursor = FindObjectOfType<PlayerCursor>();

        if (cursor.currentTool != null)
        {
            Destroy(cursor.currentTool.gameObject);
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition = new Vector3(mousePosition.x, mousePosition.y, 0f);

        Tool newTool = Instantiate(allTools[index].gameObject, mousePosition, Quaternion.identity, cursor.transform).GetComponent<Tool>();

        cursor.currentTool = newTool;

        // Close tab
        mainWindow.SetWindowOpen(false);

        // Play sound
        FindObjectOfType<AudioController>().Play("MenuTap");
    }

    // SetTool, but for manual use (not from ToolsTab buttons)
    public void SetTool_Manual(int index)
    {
        PlayerCursor cursor = FindObjectOfType<PlayerCursor>();

        if (cursor.currentTool != null)
        {
            Destroy(cursor.currentTool.gameObject);
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition = new Vector3(mousePosition.x, mousePosition.y, 0f);

        Tool newTool = Instantiate(allTools[index].gameObject, mousePosition, Quaternion.identity, cursor.transform).GetComponent<Tool>();

        cursor.currentTool = newTool;
    }
}
