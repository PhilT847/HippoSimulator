using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Pointer in the main menu. Not a Tool, but has similar usage
public class MenuPointer : MonoBehaviour
{
    // Make the actual cursor invisible and use this tool
    private void Awake()
    {
        // Make base cursor invisible
        Cursor.visible = false;
    }

    private void Update()
    {
        // Follow cursor
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePosition.x, mousePosition.y, 0f);
    }
}
