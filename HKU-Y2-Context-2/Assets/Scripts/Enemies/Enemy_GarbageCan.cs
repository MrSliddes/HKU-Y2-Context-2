using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_GarbageCan : MonoBehaviour, IDamageable
{
    public float health = 10;
    public float damage = 1;
    public float attackRange = 5;
    public float attackSpeed = 2.5f;
    public float projectileForce = 10;

    public GameObject prefabProjectile;
    public Transform projectileOrigin;

    private float attackSpeedTimer;
    private bool playerEnterdRange = false;
    private Transform player;


    // Start is called before the first frame update
    void Start()
    {
        // Get
        player = GameObject.FindWithTag("Player").transform; if(player == null) Debug.LogWarning("No player detected!");

        // Set
        attackSpeedTimer = attackSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        // If player is close enough start shooting
        if(Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            if(!playerEnterdRange)
            {
                playerEnterdRange = true;
                attackSpeedTimer = 0;
            }

            if(attackSpeedTimer > 0)
            {
                attackSpeedTimer -= Time.deltaTime;
            }
            else
            {
                // Shoot
                GameObject a = Instantiate(prefabProjectile, projectileOrigin.position, Quaternion.identity);
                Vector3 rot = Vector3.RotateTowards(projectileOrigin.position, player.position, 999, 0.0f);
                a.GetComponent<Rigidbody>().AddRelativeForce(rot * projectileForce);
                attackSpeedTimer = attackSpeed;
            }
        }
        else
        {
            playerEnterdRange = false;
        }
    }

    public void ReceiveDamage(float amount)
    {
        health -= amount;
        if(health <= 0)
        {
            Destroy(gameObject);
        }        
    }

    private void OnCollisionEnter(Collision collision)
    {
        EnemyProjectile ep = collision.transform.GetComponent<EnemyProjectile>();
        if(ep != null && ep.canHurtEnemies)
        {
            // 1 hit
            Destroy(gameObject);
        }
    }
}
