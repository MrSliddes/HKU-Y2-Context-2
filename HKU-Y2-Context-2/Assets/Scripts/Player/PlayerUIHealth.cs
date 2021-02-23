using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIHealth : MonoBehaviour
{
    public GameObject[] healtUI;

    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        // Get
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null)
        {
            Debug.LogWarning("No player detected!");
            return;
        }

        // Display player health (discusting code yea ik)
        healtUI[0].SetActive(player.health > 0);
        healtUI[1].SetActive(player.health > 1);
        healtUI[2].SetActive(player.health > 2);
    }
}
