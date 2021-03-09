using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{
    public string sceneToLoad;

    public GameObject[] objectsToHideOnStart;

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject item in objectsToHideOnStart)
        {
            item.SetActive(false);
        }

        if(sceneToLoad == "")
        {
            Debug.LogError("No scene name set! Please set a scene name to load");
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
