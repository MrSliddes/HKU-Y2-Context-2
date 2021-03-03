using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEnemy : MonoBehaviour
{
    public bool onlyOnce = true;
    public float damage = 1;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 0.2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<IDamageable>() != null)
        {
            // Deal damage
            other.GetComponent<IDamageable>().ReceiveDamage(damage);
            Destroy(gameObject);
        }
    }
}
