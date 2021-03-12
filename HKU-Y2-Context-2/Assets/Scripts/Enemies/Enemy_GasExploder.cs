using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_GasExploder : Enemy
{
    [Header("Enemy Type Values")]
    public float aggroRange = 2;
    public float attackRange = 2;
    public float attackSpeed = 3;
    public LayerMask layerMaskPlayer;
    public ParticleSystem particleSystemExplosion;

    private Player player;
    private float attackSpeedTimer;
    private bool enterdAttackRange = false;

    public override void Start()
    {
        base.Start();
        player = FindObjectOfType<Player>();
        if(player == null) Debug.LogWarning("No player found!");

        // Set
        attackSpeedTimer = attackSpeed;
        particleSystemExplosion.Stop();
    }

    public override void ContactWithPlayer(Collision collision)
    {
        // No contact damage
        //base.ContactWithPlayer(collision);
    }

    public override void EnemyStatePatrolling()
    {
        base.EnemyStatePatrolling();

        if(Vector3.Distance(transform.position, player.transform.position) < aggroRange)
        {
            if(!enterdAttackRange)
            {
                enterdAttackRange = true;
                attackSpeedTimer = attackSpeed;
            }

            if(attackSpeedTimer > 0) attackSpeedTimer -= Time.deltaTime;
            else
            {
                // Attack
                Collider[] collisions = Physics.OverlapSphere(transform.position, attackRange, layerMaskPlayer);
                foreach(Collider item in collisions)
                {
                    item.GetComponent<Player>().ReceiveDamage(damage, transform);
                }
                StartCoroutine(ExplosionAsync());

                attackSpeedTimer = attackSpeed;
            }
        }
    }

    private IEnumerator ExplosionAsync()
    {
        particleSystemExplosion.Play();
        yield return new WaitForSeconds(0.5f);
        particleSystemExplosion.Stop();
        yield break;
    }

    public override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
