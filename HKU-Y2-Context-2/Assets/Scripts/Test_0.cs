using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_0 : MonoBehaviour
{
    public GameObject[] prefabBuildings;
    public int currentBuildingIndex;

    public LayerMask layerMaskDefault;

    public Transform previewTransform;
    public GameObject previewBuilding;

    // Start is called before the first frame update
    void Start()
    {
        previewBuilding = Instantiate(prefabBuildings[currentBuildingIndex]);
        currentBuildingIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Change building
        if(Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            currentBuildingIndex++;
            if(currentBuildingIndex > prefabBuildings.Length - 1) currentBuildingIndex = 0;
            GameObject a = previewBuilding;
            previewBuilding = null;
            Destroy(a);
            previewBuilding = Instantiate(prefabBuildings[currentBuildingIndex]);
        }
        else if(Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            currentBuildingIndex--;
            if(currentBuildingIndex < 0) currentBuildingIndex = prefabBuildings.Length - 1;
            GameObject a = previewBuilding;
            previewBuilding = null;
            Destroy(a);
            previewBuilding = Instantiate(prefabBuildings[currentBuildingIndex]);
        }
        

        // Show review model
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskDefault))
        {
            // Do something with the object that was hit by the raycast.
            previewTransform.position = hit.point;
            previewBuilding.transform.position = hit.point;
        }

        if(Input.GetMouseButtonDown(0))
        {
            if(Physics.Raycast(ray, out hit))
            {
                // Place current house
                GameObject a = Instantiate(prefabBuildings[currentBuildingIndex], hit.point, Quaternion.identity);
                a.GetComponent<Building>().Placed();
            }
        }
    }
}
