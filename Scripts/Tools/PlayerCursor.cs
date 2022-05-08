using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCursor : MonoBehaviour
{
    public Tool baseTool;

    public Tool currentTool;

    public InfoRevealer currentRevealer;

    // Start with basic cursor tool
    private void Start()
    {
        // Make base cursor invisible
        Cursor.visible = false;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition = new Vector3(mousePosition.x, mousePosition.y, 0f);

        Tool newTool = Instantiate(baseTool.gameObject, mousePosition, Quaternion.identity, transform).GetComponent<Tool>();

        currentTool = newTool;
    }

    // Search for info tabs
    // If activated, show info based on active information revealer
    private void Update()
    {
        // Follow mouse
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition = new Vector3(mousePosition.x, mousePosition.y, 0f);

        transform.position = mousePosition;

        //GetComponent<Rigidbody2D>().velocity = (mousePosition - transform.position) * 12f;

        if (Input.GetMouseButtonDown(1))
        {
            if(currentRevealer != null)
            {
                currentRevealer.OpenInfo();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<InfoRevealer>())
        {
            // Hide current revealer, if applicable
            if (currentRevealer != null)
            {
                currentRevealer.HideRevealer();

                currentRevealer = null;
            }

            currentRevealer = collision.GetComponent<InfoRevealer>();

            currentRevealer.ShowRevealer();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<InfoRevealer>() && currentRevealer != null)
        {
            currentRevealer.HideRevealer();

            currentRevealer = null;
        }
    }
}
