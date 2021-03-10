using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Instance { get; set; }

    public GameObject escapeMenu;
    public GameObject[] healthFull;
    public Animator animatorLevelTransition;
    public GameObject gameOverScreen;

    private Player player;

    private bool isGameOver;

    private void Awake()
    {
        Instance = this;
    }

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

        if(isGameOver)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    public static void LevelTransitionOpening()
    {
        Instance.animatorLevelTransition.Play("Anim_UI_LevelTransition_Open");
    }

    public static void LevelTransitionClose()
    {
        Instance.animatorLevelTransition.Play("Anim_UI_LevelTransition_Close");
    }

    public static void ShowGameOverScreen()
    {
        Instance.gameOverScreen.SetActive(true);
        Instance.isGameOver = true;
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
