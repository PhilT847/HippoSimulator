using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphPoint : MonoBehaviour
{
    public LineRenderer pointLine;

    // Use a line to connect one point and the next
    // The last point just "connects" to itself
    public void ConnectPointTo(GraphPoint nextPoint)
    {
        Vector3[] bothPoints = new Vector3[2];

        bothPoints[0] = GetComponent<RectTransform>().position;

        if(nextPoint != null)
        {
            bothPoints[1] = nextPoint.GetComponent<RectTransform>().position;
        }
        else
        {
            bothPoints[1] = GetComponent<RectTransform>().position;
        }

        // Set z to zero so they show on the UI
        bothPoints[0].z = 0f;
        bothPoints[1].z = 0f;

        pointLine.SetPositions(bothPoints);
    }
}
