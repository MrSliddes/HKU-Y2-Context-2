using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePad : MonoBehaviour
{
    public float bounceForce = 5;

    // Start is called before the first frame update
    void Start()
    {
        // Set
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Player"))
        {
            if(collision.transform.position.y > transform.position.y)
            {
                collision.transform.GetComponent<Rigidbody>().AddForce(Vector3.up * bounceForce);
            }
        }
    }
}
