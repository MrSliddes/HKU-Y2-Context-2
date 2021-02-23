using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCanvas : MonoBehaviour
{
    public static PlayerCanvas Instance { get; set; }

    public GameObject escapeMenu;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        escapeMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        ToggleEscapeMenu();
    }

    private void ToggleEscapeMenu()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            escapeMenu.SetActive(!escapeMenu.activeSelf);
        }
    }
}
