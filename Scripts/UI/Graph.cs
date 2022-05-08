using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    // Graph comparing all statistics
    public GraphLine[] allLines;

    // Display only certain lines when opening the Graph
    // 0-2 (hippo, blue, red) are displayed. Others are hidden
    public void DisplayRelevantLines()
    {
        for(int i = 0; i < allLines.Length; i++)
        {
            if(i < 3)
            {
                allLines[i].SetLineDisplay(true);
            }
            else
            {
                allLines[i].SetLineDisplay(false);
            }
        }
    }
}
