using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    public string flowerName;

    // Ability of this flower to beat out other flowers when both try to grow somewhere
    public int competitivePower;

    public Sprite appearanceSprite;

    // % chance of spreading to neighboring tiles after a generation
    public float chanceOfSpreading;

    // % chance that the flower dies upon trampling
    public float chanceOfTrampleDeath;

    // % effect of trampling on spread chance
    public float trampleSpreadMod;

    // Remove a flower from the selected tile
    public void KillFlower(Tile originTile)
    {
        originTile.flowerSprite.sprite = null;

        originTile.currentFlower = null;
    }
}
