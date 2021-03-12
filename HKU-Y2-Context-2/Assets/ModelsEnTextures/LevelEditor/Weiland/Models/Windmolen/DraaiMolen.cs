using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraaiMolen : MonoBehaviour
{
    public float turnSpeed = 10f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0f, turnSpeed, 0f) * Time.deltaTime);
    }
}
