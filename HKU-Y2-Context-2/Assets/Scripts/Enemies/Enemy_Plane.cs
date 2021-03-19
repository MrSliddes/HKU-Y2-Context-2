using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Plane : MonoBehaviour
{
    public bool moveRight;
    public float damage = 1;
    public float speed = 3;

    public Rigidbody rb;
    public SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
            spriteRenderer.flipX = !moveRight;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(moveRight)
        {
            rb.MovePosition(transform.position + Vector3.right * speed * Time.deltaTime);
        }
        else
        {
            rb.MovePosition(transform.position + Vector3.left * speed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Player"))
        {
            collision.transform.GetComponent<Player>().ReceiveDamage(damage, transform);
        }
        Destroy(gameObject);
    }
}
