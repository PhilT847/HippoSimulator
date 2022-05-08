using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tab : MonoBehaviour
{
    public ControlWindow mainWindow;

    // Tab that appears when selected
    public GameObject tabObject;

    public abstract void OpenTab();
    public abstract void CloseTab();
}
