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
    public Animator animator;
    public Transform animatorObject;

    private Player player;
    private float attackSpeedTimer;
    private bool enterdAttackRange = false;
    private bool didAttack = false;
    private bool finishedAttack = true;

    public override void Start()
    {
        base.Start();
        player = FindObjectOfType<Player>();
        if(player == null) Debug.LogWarning("No player found!");

        // Set
        attackSpeedTimer = attackSpeed;
        particleSystemExplosion.Stop();
    }

    public override void Update()
    {
        base.Update();

        // flip
        if(!spriteRenderer.flipX)
        {
            // Left
            animatorObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else
        {
            // Right
            animatorObject.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }
    }

    public override void ContactWithPlayer(Collision collision)
    {
        // No contact damage
        //base.ContactWithPlayer(collision);
    }

    public override void EnemyStateIdle()
    {
        base.EnemyStateIdle();
        animator.Play("Gas_Idle");
    }

    public override void EnemyStatePatrolling()
    {
        if(!didAttack && attackSpeedTimer == attackSpeed) base.EnemyStatePatrolling();

        if(attackSpeedTimer > 0) animator.Play("Gas_Walk");

        if(Vector3.Distance(transform.position, player.transform.position) < aggroRange || !finishedAttack)
        {
            if(!enterdAttackRange)
            {
                enterdAttackRange = true;
                attackSpeedTimer = 0;
                finishedAttack = false;
            }

            if(attackSpeedTimer > 0) attackSpeedTimer -= Time.deltaTime;
            else
            {
                
                animator.Play("Gas_attack");
                // dead dmg halfway
                if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f && animator.GetCurrentAnimatorStateInfo(0).IsName("Gas_attack"))
                {
                    if(!didAttack)
                    {
                        didAttack = true;

                        // Attack
                        Collider[] collisions = Physics.OverlapSphere(transform.position, attackRange, layerMaskPlayer);
                        foreach(Collider item in collisions)
                        {
                            item.GetComponent<Player>().ReceiveDamage(damage, transform);
                        }
                        StartCoroutine(ExplosionAsync());
                    }
                }

                if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && animator.GetCurrentAnimatorStateInfo(0).IsName("Gas_attack"))
                {
                    attackSpeedTimer = attackSpeed;
                    enterdAttackRange = false;
                    didAttack = false;
                    finishedAttack = true;
                    animator.Play("Gas_Walk");
                }

            }
        }
        else
        {
            attackSpeedTimer = attackSpeed;
            enterdAttackRange = false;
            didAttack = false;
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
