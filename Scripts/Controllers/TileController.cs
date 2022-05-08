using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public Transform tileMap;
    public List<Tile> allTiles;

    public GameObject tilePrefab; // The Tile object

    public int map_rows;
    public int map_columns;

    public Flower[] potentialFlowers;

    private void Start()
    {
        CreateTileMap();
    }

    void CreateTileMap()
    {
        int currentX = 0;
        int currentY = 0;

        for(float c = 0; c < map_columns; c++)
        {
            currentY = 0;

            for (float r = 0; r < map_rows; r++)
            {
                float tile_x = -15.75f + ((float)c * 1.499f);
                float tile_y = -8.25f + ((float)r * 1.499f);

                var newTile = Instantiate(tilePrefab, new Vector3(tile_x, tile_y, 0f), Quaternion.identity, tileMap);

                string tileName = "Tile [" + currentX + "," + currentY + "]";

                allTiles.Add(newTile.GetComponent<Tile>());

                newTile.GetComponent<Tile>().position_x = currentX;
                newTile.GetComponent<Tile>().position_y = currentY;

                // Slightly randomize flower/trample position so it looks more natural
                newTile.GetComponent<Tile>().flowerSprite.transform.localPosition = new Vector3(Random.Range(-0.4f,0.4f), Random.Range(-0.4f, 0.4f), 0f);
                newTile.GetComponent<Tile>().trampleSprite.transform.localPosition = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0f);

                newTile.GetComponent<Tile>().isLand = true;

                // Set soil quality to 1 (healthy)
                newTile.GetComponent<Tile>().soilQuality = 1;

                newTile.gameObject.name = tileName;

                currentY++;
            }

            currentX++;
        }

        // Create lake and flowers
        GenerateWaterTiles();
        AddFlowersToMap();
    }
    

    // Manually turn tiles in top right corner to water
    public void GenerateWaterTiles()
    {
        int[] waterTilesRow11 = { 14,15,16,17,18,19,20,21 };
        int[] waterTilesRow10 = { 14,15, 16, 17, 18, 19, 20, 21 };
        int[] waterTilesRow9 = { 15,16, 17, 18, 19, 20, 21 };
        int[] waterTilesRow8 = { 16,17, 18, 19, 20, 21 };
        int[] waterTilesRow7 = { 17,18, 19, 20, 21 };
        int[] waterTilesRow6 = { 18,19, 20, 21 };
        int[] waterTilesRow5 = { 18,19, 20, 21 };
        int[] waterTilesRow4 = { 19,20, 21 };

        for(int i = 0; i < 8; i++)
        {
            if(i < waterTilesRow11.Length)
            {
                GetTileAt(waterTilesRow11[i], 11).SetToWaterTile(true);
            }
            if (i < waterTilesRow10.Length)
            {
                GetTileAt(waterTilesRow10[i], 10).SetToWaterTile(true);
            }
            if (i < waterTilesRow9.Length)
            {
                GetTileAt(waterTilesRow9[i], 9).SetToWaterTile(true);
            }
            if (i < waterTilesRow8.Length)
            {
                GetTileAt(waterTilesRow8[i], 8).SetToWaterTile(true);
            }
            if (i < waterTilesRow7.Length)
            {
                GetTileAt(waterTilesRow7[i], 7).SetToWaterTile(true);
            }
            if (i < waterTilesRow6.Length)
            {
                GetTileAt(waterTilesRow6[i], 6).SetToWaterTile(true);
            }
            if (i < waterTilesRow5.Length)
            {
                GetTileAt(waterTilesRow5[i], 5).SetToWaterTile(true);
            }
            if (i < waterTilesRow4.Length)
            {
                GetTileAt(waterTilesRow4[i], 4).SetToWaterTile(true);
            }
        }
    }

    // Find a tile at a specified x and y position
    public Tile GetTileAt(int x, int y)
    {
        for(int i = 0; i < allTiles.Count; i++)
        {
            if(allTiles[i].position_x == x && allTiles[i].position_y == y)
            {
                return allTiles[i];
            }
        }

        return null;
    }

    // Add flowers to tiles
    public void AddFlowersToMap()
    {
        List<Tile> allTiles = FindObjectOfType<TileController>().allTiles;

        for (int i = 0; i < allTiles.Count; i++)
        {
            if (allTiles[i].isLand)
            {
                float diceRoll = Random.Range(0f, 1f);

                if (diceRoll < 0.3f) // ~30% are blue
                {
                    allTiles[i].AddFlower(potentialFlowers[0]);
                }
                else if (diceRoll < 0.425f) // ~12.5% are red
                {
                    allTiles[i].AddFlower(potentialFlowers[1]);
                }
                else if (allTiles[i].currentFlower != null)
                {
                    allTiles[i].currentFlower.KillFlower(allTiles[i]);
                }
            }
        }
    }
}
