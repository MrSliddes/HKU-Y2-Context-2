using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Get

        // Set
        // Check if there are more than 1
        if(FindObjectsOfType<LevelStart>().Length > 1)
        {
            Debug.LogError("There are multiple Level Start Objects, there should only be one!");
            Destroy(gameObject);
        }
        else
        {
            // Set player pos
            FindObjectOfType<Player>().transform.position = transform.position;
            // Play animation
            PlayerUI.LevelTransitionOpening();
            Destroy(gameObject);
        }    
    }
}
