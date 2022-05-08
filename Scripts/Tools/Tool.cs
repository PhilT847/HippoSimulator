using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tool : MonoBehaviour
{
    // The tool does not operate when clicking on buttons or operating the Control Window
    public bool IsTouchingWindow()
    {
        // Check cursor position
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // If the window's open or the tool is too low, do not activate
        if (FindObjectOfType<ControlWindow>().windowOpen || 
            (!FindObjectOfType<ControlWindow>().windowOpen && mousePosition.y < -7.5f && mousePosition.x > 2f))
        {
            return true;
        }

        // Check for pressing the fast forward button
        if (transform.position.y > 6.5f && transform.position.x > 14.5f)
        {
            return true;
        }

        // If the info window is open and the tool's touching it, don't activate
        if (FindObjectOfType<InfoTab>().tabOpen &&
            (mousePosition.y > -1f && mousePosition.x < -11f))
        {
            return true;
        }

        return false;
    }
}
