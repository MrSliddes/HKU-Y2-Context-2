using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneIn : MonoBehaviour
{
    public string sceneToLoadName;
    public float timeUntilLoad = 1;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(timeUntilLoad);
        SceneManager.LoadSceneAsync(sceneToLoadName, LoadSceneMode.Single);
        yield break;
    }
}
