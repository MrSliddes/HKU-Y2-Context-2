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
    public SpriteRenderer spriteRenderer;

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

                // Calc angle 
                Vector3 vel = CalculateVelocity(player.position, projectileOrigin.position, 1f);

                a.GetComponent<Rigidbody>().velocity = vel;
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

        StartCoroutine(DisplayHitAsync());
    }

    private IEnumerator DisplayHitAsync()
    {
        // Sprite flikker
        Color f = Color.red;
        Color c = spriteRenderer.color;
        spriteRenderer.color = f;
        yield return new WaitForSeconds(0.15f);
        spriteRenderer.color = c;
        yield break;
    }

    private void OnCollisionEnter(Collision collision)
    {
        EnemyProjectile ep = collision.transform.GetComponent<EnemyProjectile>();
        if(ep != null && ep.canHurtEnemies)
        {
            // 1 hit
            //Destroy(gameObject);
        }
    }

    private Vector3 CalculateVelocity(Vector3 target, Vector3 orgin, float time)
    {
        Vector3 distance = target - orgin;
        Vector3 distanceXz = distance;
        distanceXz.y = 0;

        float sY = distance.y;
        float sXz = distanceXz.magnitude;

        float Vxz = sXz * time;
        float Vy = (sY / time) + (0.5f * Mathf.Abs(Physics.gravity.y) * time);

        Vector3 result = distanceXz.normalized;
        result *= Vxz;
        result.y = Vy;
        return result;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
