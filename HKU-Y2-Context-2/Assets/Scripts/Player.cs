using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float movementSpeed = 5f;

    [Header("Required Components")]
    public Rigidbody rb;

    private Vector3 input;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
    }

    private void FixedUpdate()
    {
        // Move rb
        //rb.AddForce(input * movementSpeed * Time.fixedDeltaTime, ForceMode.Force);
        rb.velocity = new Vector3(input.x * movementSpeed, rb.velocity.y);
        // No input = stop
        //if(input.x == 0) rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
    }

    private void PlayerInput()
    {
        input.x = Input.GetAxisRaw("Horizontal");

        
    }
}
