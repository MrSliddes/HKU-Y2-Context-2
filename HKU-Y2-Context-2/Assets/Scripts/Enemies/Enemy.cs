using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public float health = 3;
    public float damage = 1;
    public float movementSpeed = 1;
    public float idleTime = 3;
    [SerializeField] private PatrolAxis patrolAxis;
    public Vector3 patrolPointA = new Vector3(-4, 0, 0);
    public Vector3 patrolPointB = new Vector3(4, 0, 0);

    public EnemyState enemyState;
    public SpriteRenderer spriteRenderer;

    protected bool hasEnterdNewState = false;
    private bool patrolToLeft;
    private float idleTimer;
    private Vector3 orginPosition;
    private CapsuleCollider capsuleCollider;

    // Start is called before the first frame update
    public virtual void Start()
    {
        // Get
        capsuleCollider = GetComponent<CapsuleCollider>();

        // Set
        orginPosition = transform.position;
        EnterNewEnemyState(EnemyState.idle);
    }

    // Update is called once per frame
    public virtual void Update()
    {
        UpdateEnemyState();
    }

    /// <summary>
    /// Triggerd when player comes in contact
    /// </summary>
    /// <param name="collision"></param>
    public virtual void ContactWithPlayer(Collision collision)
    {
        // Deal damage to player (and give knockback?)
        collision.transform.GetComponent<Player>().ReceiveDamage(damage, transform);
    }

    public virtual void ContactWithEnemy(Collision collision)
    {

    }

    public virtual void EnterNewEnemyState(EnemyState newState)
    {
        enemyState = newState;
        hasEnterdNewState = false;
    }

    #region Virtual Enemy States

    public virtual void EnemyStateIdle()
    {
        // Enter
        if(!hasEnterdNewState)
        {
            hasEnterdNewState = true;
            idleTimer = idleTime;
        }
        // Update
        idleTimer -= Time.deltaTime;

        // Exit
        if(idleTimer <= 0)
        {
            EnterNewEnemyState(EnemyState.patrolling);
        }
    }

    public virtual void EnemyStatePatrolling()
    {
        // Enter
        if(!hasEnterdNewState)
        {
            hasEnterdNewState = true;
            patrolToLeft = true;
        }
        // Update
        if(patrolToLeft)
        {
            spriteRenderer.flipX = true;
            switch(patrolAxis)
            {
                case PatrolAxis.horizontal:
                    // Move to A
                    transform.position = new Vector3(Mathf.MoveTowards(transform.position.x, orginPosition.x + patrolPointA.x, movementSpeed * Time.deltaTime), transform.position.y, transform.position.z);
                    // If close enough
                    if(Mathf.Abs(transform.position.x - (orginPosition.x + patrolPointA.x)) <= 1) patrolToLeft = false;
                    // If over the edge
                    if(!IsGroundedLeftSide()) patrolToLeft = false;
                    break;
                case PatrolAxis.vertical:
                    // Move to A
                    transform.position = new Vector3(transform.position.x, Mathf.MoveTowards(transform.position.y, orginPosition.y + patrolPointA.y, movementSpeed * Time.deltaTime), transform.position.z);
                    // If close enough
                    if(Mathf.Abs(transform.position.y - orginPosition.y + patrolPointB.y) <= 1) patrolToLeft = false;
                    break;
                case PatrolAxis.precise:
                    // Move to A
                    transform.position = Vector3.MoveTowards(transform.position, orginPosition + patrolPointA, movementSpeed * Time.deltaTime);
                    // Check if close enough
                    if(Vector3.Distance(transform.position, orginPosition + patrolPointA) <= 1f) patrolToLeft = false;
                    break;
                default:
                    break;
            }
        }
        else
        {
            spriteRenderer.flipX = false;
            switch(patrolAxis)
            {
                case PatrolAxis.horizontal:
                    // Move to B
                    transform.position = new Vector3(Mathf.MoveTowards(transform.position.x, orginPosition.x + patrolPointB.x, movementSpeed * Time.deltaTime), transform.position.y, transform.position.z);
                    // If close enough or over the edge
                    if(Mathf.Abs(transform.position.x - (orginPosition.x + patrolPointB.x)) <= 1 || !IsGroundedRightSide())
                    {
                        // Exit
                        EnterNewEnemyState(EnemyState.idle);
                        return;
                    }
                    break;
                case PatrolAxis.vertical:
                    // Move to B
                    transform.position = new Vector3(transform.position.x, Mathf.MoveTowards(transform.position.y, orginPosition.y + patrolPointB.y, movementSpeed * Time.deltaTime), transform.position.z);
                    // If close enough
                    if(Mathf.Abs(transform.position.y - orginPosition.y + patrolPointB.y) <= 1)
                    {
                        // Exit
                        EnterNewEnemyState(EnemyState.idle);
                        return;
                    }
                    break;
                case PatrolAxis.precise:
                    // If over the edge
                    if(IsGroundedRightSide())
                    {
                        // Move to B
                        transform.position = Vector3.MoveTowards(transform.position, orginPosition + patrolPointB, movementSpeed * Time.deltaTime);
                    }
                    else
                    {
                        // Not grounded, exit
                        EnterNewEnemyState(EnemyState.idle);
                    }
                    break;
                default:
                    break;
            }
        }

        // Exit
        if(!patrolToLeft && Vector3.Distance(transform.position, orginPosition + patrolPointB) <= 1f)
        {
            // Done patrolling
            EnterNewEnemyState(EnemyState.idle);
        }
    }

    public virtual void EnemyStateWalking()
    {
        // Enter
        if(!hasEnterdNewState)
        {
            hasEnterdNewState = true;
        }
        // Update

        // Exit
    }

    public virtual void EnemyStateChasing()
    {
        // Enter
        if(!hasEnterdNewState)
        {
            hasEnterdNewState = true;
        }
        // Update

        // Exit
    }

    public virtual void EnemyStateAttacking()
    {
        // Enter
        if(!hasEnterdNewState)
        {
            hasEnterdNewState = true;
        }
        // Update

        // Exit
    }

    public virtual void EnemyStateDead()
    {
        // Enter
        if(!hasEnterdNewState)
        {
            hasEnterdNewState = true;
            Destroy(gameObject);
        }
        // Update

        // Exit
    }

    #endregion

    public virtual void UpdateEnemyState()
    {
        switch(enemyState)
        {
            case EnemyState.idle:       EnemyStateIdle();       break;
            case EnemyState.patrolling: EnemyStatePatrolling(); break;
            case EnemyState.walking:    EnemyStateWalking();    break;
            case EnemyState.chasing:    EnemyStateChasing();    break;
            case EnemyState.attacking:  EnemyStateAttacking();  break;
            case EnemyState.dead:       EnemyStateDead();       break;
            default: Debug.Log("EnemyState switch error!");     break;
        }
    }

    #region Damage Receiving

    public virtual void ReceiveDamage(float damage)
    {
        health -= damage;
        if(health <= 0) EnterNewEnemyState(EnemyState.dead);
        StopCoroutine(DisplayHitAsync());
        StartCoroutine(DisplayHitAsync());
    }

    public virtual IEnumerator DisplayHitAsync()
    {
        // Sprite flikker
        Color f = Color.red;
        Color c = spriteRenderer.color;
        spriteRenderer.color = f;
        yield return new WaitForSeconds(0.15f);
        spriteRenderer.color = c;
        yield break;
    }

    #endregion

    #region Raycasts

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, capsuleCollider.bounds.extents.y + 0.01f);
    }

    private bool IsGroundedLeftSide()
    {
        return Physics.Raycast(transform.position - new Vector3(0.1f, 0, 0), -Vector3.up, capsuleCollider.bounds.extents.y + 0.01f);
    }

    private bool IsGroundedRightSide()
    {
        return Physics.Raycast(transform.position + new Vector3(0.1f, 0, 0), -Vector3.up, capsuleCollider.bounds.extents.y + 0.01f);
    }

    #endregion

    

    public virtual void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Player"))
        {
            ContactWithPlayer(collision);
        }
        else if(collision.transform.GetComponent<Enemy>() != null)
        {
            ContactWithEnemy(collision);
        }
    }


    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;

        if(!Application.isPlaying) orginPosition = transform.position;
        Gizmos.DrawLine(orginPosition + patrolPointA, orginPosition + patrolPointB);
    }

    public enum EnemyState
    { 
        idle,
        patrolling,
        walking,
        chasing,
        attacking,
        dead
    }

    private enum PatrolAxis
    {
        horizontal,
        vertical,
        precise
    }
}
