using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Vuilniszak : MonoBehaviour, IDamageable
{
    public float health = 1;
    public float damage = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Player"))
        {
            collision.transform.GetComponent<Player>().ReceiveDamage(damage, transform);
        }
    }

    public void ReceiveDamage(float amount)
    {
        health -= amount;
        if(health <= 0) Destroy(gameObject);
    }
}
