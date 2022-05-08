using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphLine : MonoBehaviour
{
    public Sprite pointSymbol;
    public Color pointColor;

    public GameObject pointObject;

    public bool isShowing = true;

    // Button that shows/hides this line
    public Button enablingButton;

    // Allows for connecting points using lines
    public List<GraphPoint> allPoints;

    public void AddPoint(float value, int year)
    {
        // Instantiate a new point within this line's transform
        // Keeping all points within a graphLine helps when making them all invisible at once
        var newPoint = Instantiate(pointObject, transform.position, Quaternion.identity, transform);

        // Set symbol/color
        newPoint.GetComponent<Image>().sprite = pointSymbol;
        newPoint.GetComponent<Image>().color = pointColor;
        newPoint.GetComponent<GraphPoint>().pointLine.startColor = pointColor;
        newPoint.GetComponent<GraphPoint>().pointLine.endColor = pointColor;

        allPoints.Add(newPoint.GetComponent<GraphPoint>());

        float x_pos = 0f + ((year - 1) * 59.5f); // values from 1-10, and x from 0 -> 595
        float y_pos = 0f + (value * 3.95f); // values from 0-100, and y from 0 -> 395

        newPoint.GetComponent<RectTransform>().localPosition = new Vector3(x_pos, y_pos, 0f);

        AddLines();
    }

    public void ToggleLineDisplay()
    {
        isShowing = !isShowing;

        // Play sound
        FindObjectOfType<AudioController>().Play("MenuTap");

        // Make button look en-/disabled based on showing/hiding
        if (isShowing)
        {
            enablingButton.GetComponent<Image>().color = Color.white;

            GraphPoint[] allPoints = GetComponentsInChildren<GraphPoint>(true);

            for(int i = 0; i < allPoints.Length; i++)
            {
                allPoints[i].gameObject.SetActive(true);
            }
        }
        else
        {
            enablingButton.GetComponent<Image>().color = new Color32(180, 140, 180, 255);

            GraphPoint[] allPoints = GetComponentsInChildren<GraphPoint>(true);

            for (int i = 0; i < allPoints.Length; i++)
            {
                allPoints[i].gameObject.SetActive(false);
            }
        }
    }

    public void SetLineDisplay(bool setDisplay)
    {
        isShowing = setDisplay;

        if (setDisplay)
        {
            enablingButton.GetComponent<Image>().color = Color.white;

            GraphPoint[] allPoints = GetComponentsInChildren<GraphPoint>(true);

            for (int i = 0; i < allPoints.Length; i++)
            {
                allPoints[i].gameObject.SetActive(true);
            }
        }
        else
        {
            enablingButton.GetComponent<Image>().color = new Color32(180, 140, 180, 255);

            GraphPoint[] allPoints = GetComponentsInChildren<GraphPoint>(true);

            for (int i = 0; i < allPoints.Length; i++)
            {
                allPoints[i].gameObject.SetActive(false);
            }
        }
    }

    // Create lines between each point
    void AddLines()
    {
        for(int i = 0; i < allPoints.Count; i++)
        {
            if(i < allPoints.Count - 1)
            {
                allPoints[i].ConnectPointTo(allPoints[i + 1]);
            }
        }
    }
}
