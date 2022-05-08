using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabController : MonoBehaviour
{
    public float oxygenInWater; // 0-100
    public float nutrientsInWater; // 0-100; 60 exceeds capacity, increasing algae count
    public float lastNutrientsInWater; // used for the graph, since nutrients reset each round

    // Abundance percentages; 0-100 for each
    public float hippoHealth;
    public float flower1_Health;
    public float flower2_Health;
    public float algaeCount;
    public float fishCount;

    // Soil percentages
    public float soilRejuvenatedPercentage;
    public float soilHealthyPercentage;
    public float soilUnhealthyPercentage;
    public float soilDestroyedPercentage;
    public float overallSoilHealthPercentage;

    public SpriteRenderer algaeSprite;

    // The graph shown in the Graph menu
    public Graph gameGraph;

    // Update lab data
    // On the first round, do not update nutrients/oxygen; just show base stats
    public void UpdateLabData(bool firstYear)
    {
        // Alter statistics if advancing a year
        if (!firstYear)
        {
            TurnNutrientsIntoAlgae();
            AlterAvailableOxygen();
        }

        CalculateSoilHealth();
        CalculateOtherHealth();

        FindObjectOfType<LabTab>().UpdateLabTab();
        UpdateGraph();
    }

    // Add nutrients from a resting hippo
    public void AddNutrients(float num)
    {
        nutrientsInWater += num;

        if(nutrientsInWater > 100f)
        {
            nutrientsInWater = 100f;
        }

        // Update lab slider for nutrients in real time
        FindObjectOfType<LabTab>().nutrientSlider.value = nutrientsInWater;
    }

    void TurnNutrientsIntoAlgae()
    {
        // Nutrients are created actively in Hippo's resting behavior
        if(nutrientsInWater >= 50f)
        {
            // Add 1 algae for each percentage point above 50%
            algaeCount += nutrientsInWater - 50f;
        }

        // Cap at 100 and 0
        if (algaeCount > 100f)
        {
            algaeCount = 100f;
        }
        else if(algaeCount < 0f)
        {
            algaeCount = 0f;
        }

        // Set algae sprite in the water based on current count
        algaeSprite.color = new Color(1f,1f,1f, algaeCount / 100f);

        // Save then reset nutrient count (for graphing purposes)
        lastNutrientsInWater = nutrientsInWater;
        nutrientsInWater = 0f;
    }

    void AlterAvailableOxygen()
    {
        // Oxygen varies slightly each year, but is reduced by algae
        oxygenInWater += Random.Range(-2f, 2f);
        
        // Once algae reaches a certain point, reduce oxygen
        if(algaeCount > 10)
        {
            oxygenInWater -= algaeCount * 0.5f;
        }

        // Cap oxygen at 0-100
        if (oxygenInWater < 0f)
        {
            oxygenInWater = 0f;
        }
        else if(oxygenInWater > 100f)
        {
            oxygenInWater = 100f;
        }

        // At low oxygen, fish die (1% per point below 40%)
        if(oxygenInWater <= 40f)
        {
            float percentagePointsBelow40 = (oxygenInWater - 40f) * -1f;

            fishCount -= percentagePointsBelow40;

            if(fishCount < 0f)
            {
                fishCount = 0f;
            }
        }
        else // Otherwise, slightly replenish fish population (5-10%)
        {
            if(fishCount < 90f) // At low levels, replenish.
            {
                fishCount += Random.Range(5f, 10f);
            }
            else // At high levels, randomize slightly
            {
                fishCount += Random.Range(-4f, 2f);
            }

            if(fishCount > 100f)
            {
                fishCount = 100f;
            }
        }
    }

    void CalculateSoilHealth()
    {
        List<Tile> allTiles = FindObjectOfType<TileController>().allTiles;

        int totalSoilCount = 0;
        int totalRejuvenated = 0;
        int totalHealthy = 0;
        int totalUnhealthy = 0;
        int totalDestroyed = 0;
        int totalScore = 0; // score used for overall health

        for(int i = 0; i < allTiles.Count; i++)
        {
            if (allTiles[i].isLand)
            {
                totalSoilCount++;

                switch(allTiles[i].soilQuality)
                {
                    case 0: // Rejuvenated; 100% score
                        totalRejuvenated++;
                        totalScore += 5;
                        break;
                    case 1: // Healthy: 80% score
                        totalHealthy++;
                        totalScore += 4;
                        break;
                    case 2: // Unhealthy: 20% score
                        totalUnhealthy++;
                        totalScore += 1;
                        break;
                    case 3: // Destroyed: 0% score
                        totalDestroyed++;
                        totalScore += 0;
                        break;
                }
            }
        }

        soilRejuvenatedPercentage = (float)totalRejuvenated / totalSoilCount;
        soilHealthyPercentage = (float)totalHealthy / totalSoilCount;
        soilUnhealthyPercentage = (float)totalUnhealthy / totalSoilCount;
        soilDestroyedPercentage = (float)totalDestroyed / totalSoilCount;
        overallSoilHealthPercentage = (float)totalScore / (totalSoilCount * 5);
    }

    // Health of hippos and flowers
    public void CalculateOtherHealth()
    {
        // Hippo: 4.2% per hippo, >100% by 24 (max)
        hippoHealth = FindObjectOfType<GameController>().hippoHerd.Count * 4.2f;

        // Cap at 100%
        if(hippoHealth > 100f)
        {
            hippoHealth = 100f;
        }

        // Flowers: Cap at 100% at 50... 2% per flower
        List<Tile> allTiles = FindObjectOfType<TileController>().allTiles;

        flower1_Health = 0f;
        flower2_Health = 0f;

        // Set flower health by traversing each tile (+0.5 per flower of each type... maxed at 200)
        for(int i = 0; i < allTiles.Count; i++)
        {
            if(allTiles[i].currentFlower != null)
            {
                // 1 (blue) or 2 (red)
                if(allTiles[i].currentFlower.flowerName == "Blue")
                {
                    flower1_Health += 0.5f;

                    if(flower1_Health > 100f)
                    {
                        flower1_Health = 100f;
                    }
                }
                else
                {
                    flower2_Health += 0.5f;

                    if (flower2_Health > 100f)
                    {
                        flower2_Health = 100f;
                    }
                }
            }
        }
    }

    // Add points to the graph, using each individual value
    public void UpdateGraph()
    {
        int currentYear = FindObjectOfType<GameController>().currentYear;

        gameGraph.allLines[0].AddPoint(hippoHealth, currentYear);
        gameGraph.allLines[1].AddPoint(flower2_Health, currentYear);
        gameGraph.allLines[2].AddPoint(flower1_Health, currentYear);
        gameGraph.allLines[3].AddPoint(algaeCount, currentYear);
        gameGraph.allLines[4].AddPoint(fishCount, currentYear);
        gameGraph.allLines[5].AddPoint(lastNutrientsInWater, currentYear);
        gameGraph.allLines[6].AddPoint(oxygenInWater, currentYear);
        gameGraph.allLines[7].AddPoint(overallSoilHealthPercentage * 100f, currentYear);
    }
}
