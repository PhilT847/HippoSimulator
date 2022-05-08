using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHippo : MonoBehaviour
{
    public float currentRunTime;

    public float runSpeed;

    public SpriteRenderer[] coloredSprites;

    public Animator menuHippoAnim;

    void Update()
    {
        if(currentRunTime > 0f)
        {
            currentRunTime -= Time.deltaTime;

            transform.position = new Vector3(transform.position.x + (runSpeed * Time.deltaTime), transform.position.y, 0f);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
