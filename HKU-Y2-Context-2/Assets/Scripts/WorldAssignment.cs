using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldAssignment : MonoBehaviour
{
    public GameObject UI;

    private bool completed;

    // Start is called before the first frame update
    void Start()
    {
        UI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!completed && other.CompareTag("Player"))
        {
            UI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            UI.SetActive(false);
        }
    }

    public void Skip()
    {
        completed = true;
        UI.SetActive(false);
        Debug.Log("Skip");
    }

    public void Completed()
    {
        completed = true;
        UI.SetActive(false);
        Debug.Log("Completed");
    }
}
