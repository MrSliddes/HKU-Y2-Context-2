using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public GameObject escapeMenu;
    public GameObject[] healthFull;

    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        // Get
        player = GameObject.FindWithTag("Player").GetComponent<Player>();

        // Set
        escapeMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        ToggleEscapeMenu();

        if(player == null)
        {
            Debug.LogWarning("No player detected!");
            return;
        }

        UpdateHealtUI();
    }

    private void ToggleEscapeMenu()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            escapeMenu.SetActive(!escapeMenu.activeSelf);
        }
    }

    private void UpdateHealtUI()
    {
        healthFull[0].SetActive(player.health > 0);
        healthFull[1].SetActive(player.health > 1);
        healthFull[2].SetActive(player.health > 2);
    }
}
