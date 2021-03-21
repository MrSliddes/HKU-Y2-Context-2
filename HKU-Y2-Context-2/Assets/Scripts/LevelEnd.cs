using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelEnd : MonoBehaviour
{
    public string sceneToLoad;

    public GameObject[] objectsToHideOnStart;

    [Header("Level End Conditions")]
    public bool hasConditions = false;
    public GameObject[] objectsThatNeedToBeDestroyed;
    public TextMeshProUGUI text;

    private bool isLoading;

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

    private void Update()
    {
        if(hasConditions)
        {
            foreach(GameObject item in objectsThatNeedToBeDestroyed)
            {
                if(item != null) return;
            }

            // Change text
            text.text = "<size=60>Doel Gehaald!</size>\nVind de uitgang!";
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Player"))
        {
            Debug.Log("Col exit");
            if(!isLoading && CompletedConditions())
            {
                StartCoroutine(LoadNextScene());
            }
            else
            {
                Debug.Log("Not completed!");
            }
        }
    }

    private IEnumerator LoadNextScene()
    {
        isLoading = true;

        PlayerUI.LevelTransitionClose();
        yield return new WaitForSeconds(2);
        SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Single);
        yield break;
    }

    private bool CompletedConditions()
    {
        if(!hasConditions) return true;

        foreach(GameObject item in objectsThatNeedToBeDestroyed)
        {
            if(item != null) return false;
        }

        return true;
    }
}
