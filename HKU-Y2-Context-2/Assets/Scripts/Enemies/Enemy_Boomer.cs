using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Boomer : Enemy
{
    [Header("Boomer")]
    public float agroRange = 4;
    public float attackSpeed = 2;

    public GameObject prefabAttack;
    public GameObject boomerUI;

    private float attackSpeedTimer;
    private Transform player;


    public override void Start()
    {
        base.Start();
        player = GameObject.FindWithTag("Player").transform;
        if(boomerUI != null) boomerUI.SetActive(false);
    }

    public override void EnemyStateIdle()
    {
        base.EnemyStateIdle();

        // Exit
        if(Vector3.Distance(transform.position, player.position) <= agroRange)
        {
            EnterNewEnemyState(EnemyState.attacking);
        }
    }

    public override void EnemyStatePatrolling()
    {
        base.EnemyStatePatrolling();

        // Exit if player in range
        if(Vector3.Distance(transform.position, player.position) <= agroRange)
        {
            EnterNewEnemyState(EnemyState.attacking);
        }
    }

    public override void EnemyStateAttacking()
    {
        // Enter
        if(!hasEnterdNewState)
        {
            hasEnterdNewState = true;
        }
        // Update
        // Shoot blast
        if(attackSpeedTimer < 0)
        {
            // Attack
            if(player.position.x > transform.position.x)
            {
                // Right
                GameObject a = Instantiate(prefabAttack, transform.position + new Vector3(1.5f, 0.5f, 0), Quaternion.identity);
                a.GetComponent<Enemy_Plane>().moveRight = true;
            }
            else
            {
                // Left
                GameObject a = Instantiate(prefabAttack, transform.position + new Vector3(-1.5f, 0.5f, 0), Quaternion.identity);
                a.GetComponent<Enemy_Plane>().moveRight = false;
            }

            attackSpeedTimer = attackSpeed;
        }
        else
        {
            attackSpeedTimer -= Time.deltaTime;
        }
        // Lookat player
        if(player.position.x > transform.position.x)
        {
            // Right
            spriteRenderer.flipX = false;
        }
        else
        {
            // Left
            spriteRenderer.flipX = true;
        }

        // Exit
        if(Vector3.Distance(transform.position, player.position) > agroRange)
        {
            EnterNewEnemyState(EnemyState.idle);
        }
    }

    public override void EnemyStateChasing()
    {
        // Use as text idle
        // Show text
        if(!hasEnterdNewState)
        {
            hasEnterdNewState = true;
            if(boomerUI == null)
            {
                EnterNewEnemyState(EnemyState.idle);
                return;
            }
            boomerUI.SetActive(true);
        }
    }

    public override void ReceiveDamage(float damage)
    {
        EnterNewEnemyState(EnemyState.chasing);
        StopCoroutine(DisplayHitAsync());
        StartCoroutine(DisplayHitAsync());
    }

    public void BoomerAwnserGood()
    {
        Debug.Log("Good");
    }

    public void BoomerAwnserBad()
    {
        Debug.Log("Bad");
    }

    public override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, agroRange);
    }
}
