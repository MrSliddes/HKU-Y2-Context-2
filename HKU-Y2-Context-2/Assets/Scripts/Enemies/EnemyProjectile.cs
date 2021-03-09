using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float lifeTime = 3;
    public float damage;
    public bool canHurtEnemies = false;

    public SpriteRenderer spriteRenderer;
    public Sprite[] randomSprite;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 3);

        // Set random sprite if possible
        if(spriteRenderer != null)
        {
            if(randomSprite.Length > 1)
            {
                spriteRenderer.sprite = randomSprite[Random.Range(0, randomSprite.Length)];
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.transform.GetComponent<Player>() != null)
        {
            other.transform.GetComponent<Player>().ReceiveDamage(damage, transform);
            Destroy(gameObject);
        }
        else if(other.transform.GetComponent<IDamageable>() != null && canHurtEnemies)
        {
            other.transform.GetComponent<IDamageable>().ReceiveDamage(damage);
        }
    }
}
