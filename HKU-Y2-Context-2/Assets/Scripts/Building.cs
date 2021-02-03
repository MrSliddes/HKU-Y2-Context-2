using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Building : MonoBehaviour
{
    public BuildingType buildingType;
    public float buildingRange = 1;

    public LayerMask layerMaskBuildings;
    public TextMeshPro buildingScoreText;

    public bool isPlaced = false;
    public float buildingScore;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlaced) return;

        buildingScore = 0; // reset

        // Check surrounding buildings
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1, layerMaskBuildings);
        for(int i = 0; i < colliders.Length; i++)
        {
            // Check if building
            if(colliders[i].GetComponent<Building>() != null)
            {
                Building other = colliders[i].GetComponent<Building>();

                // is building now get score
                switch(buildingType)
                {
                    case BuildingType.house:
                        switch(other.buildingType)
                        {
                            case BuildingType.house: buildingScore += 1; break;
                            case BuildingType.centrale: buildingScore -= 2; break;
                            case BuildingType.greenEnergy: buildingScore -= 1; break;
                            default:
                                break;
                        }
                        break;
                    case BuildingType.powerTower:
                        break;
                    case BuildingType.centrale:
                        break;
                    case BuildingType.greenEnergy:
                        buildingScore += 5;
                        break;
                    default:
                        break;
                }
            }
        }

        // Update score
        buildingScoreText.text = buildingScore.ToString();
    }

    public void Placed()
    {
        isPlaced = true;
        buildingScoreText.text = "";
    }
}

public enum BuildingType
{
    house,
    powerTower,
    centrale,
    greenEnergy
}