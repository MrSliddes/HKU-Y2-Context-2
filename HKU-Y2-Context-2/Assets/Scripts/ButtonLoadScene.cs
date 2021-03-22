using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonLoadScene : MonoBehaviour
{
    public string sceneToLoadName;

    public GameObject LoadingScreen;


    public void Load()
    {
        LoadingScreen?.SetActive(true);
        SceneManager.LoadSceneAsync(sceneToLoadName, LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT GAME");
        Application.Quit();
    }
}
