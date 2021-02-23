using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float health = 3;
    public float movementSpeed = 5f;
    public float jumpForce;
    public float gravityForce = 1;
    public float invincibleTime = 3f;
    public PlayerState playerState;

    [Header("Required Components")]
    public SpriteRenderer spriteRenderer;
    [Header("Other")]
    public GameObject weaponSpritePivot;
    public GameObject laserModelPivot;

    private bool isInvincible;
    [HideInInspector] public Rigidbody rb;
    private CapsuleCollider capsuleCollider;

    /// <summary>
    /// Current input of player
    /// </summary>
    private Vector3 input;

    private bool cameOffGround;
    private bool hasEnterdNewPlayerState = false;

    // Start is called before the first frame update
    void Start()
    {
        // Get
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        // Set
        laserModelPivot.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
        UpdatePlayerState();

        // Movement
        rb.velocity = new Vector3(input.x * movementSpeed, rb.velocity.y, rb.velocity.z);

        ShootWeapon();
    }

    private void FixedUpdate()
    {
        // Move rb
        //Vector3 direction = Vector3.zero;
        //// Movement
        //direction += transform.position + new Vector3(input.x * movementSpeed, 0, 0) * Time.fixedDeltaTime;
        //// Gravity
        //if(!IsGrounded()) direction += (transform.position + (Vector3.down * gravityForce) * Time.fixedDeltaTime);
        //rb.MovePosition(direction);

        //rb.velocity = new Vector3(input.x * movementSpeed, rb.velocity.y, rb.velocity.z);
    }

    public void EnterNewPlayerState(PlayerState newState)
    {
        hasEnterdNewPlayerState = false;
        playerState = newState;
    }

    /// <summary>
    /// Triggerd when player receives damage from enemies
    /// </summary>
    /// <param name="damage"></param>
    public void ReceiveDamage(float damage) // Not triggerd by IDamageable since we dont want the player to kill himself
    {
        if(isInvincible || playerState == PlayerState.dead) return; // no damage, invis
        health -= damage;

        if(health <= 0)
        {
            EnterNewPlayerState(PlayerState.dead);
        }
        else
        {
            StopCoroutine(InvisTimeAsync());
            StartCoroutine(InvisTimeAsync());
        }
    }

    private IEnumerator InvisTimeAsync()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
        yield break;
    }

    private void UpdatePlayerState()
    {
        switch(playerState)
        {
            case PlayerState.idle:
                // Enter
                if(!hasEnterdNewPlayerState)
                {
                    hasEnterdNewPlayerState = true;
                }

                // Update

                // Exit
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    // Goto jump
                    EnterNewPlayerState(PlayerState.jump);
                }

                break;
            case PlayerState.walking:
                // Enter
                if(!hasEnterdNewPlayerState)
                {
                    hasEnterdNewPlayerState = true;
                }

                // Update

                // Exit

                break;
            case PlayerState.jump:
                // Enter
                if(!hasEnterdNewPlayerState)
                {
                    hasEnterdNewPlayerState = true;
                    rb.velocity += Vector3.up * jumpForce;
                    cameOffGround = false;
                }

                // Update
                if(!IsGrounded()) cameOffGround = true;

                // Exit
                if(cameOffGround && IsGrounded())
                {
                    EnterNewPlayerState(PlayerState.idle);
                }
                break;
            case PlayerState.dead:
                // Enter
                if(!hasEnterdNewPlayerState)
                {
                    hasEnterdNewPlayerState = true;
                }

                // Update

                // Exit

                break;
            default: Debug.LogWarning("Wrong playerstate"); break;
        }
    }

    private void FlipSprite()
    {
        if(input.x > 0)
        {
            spriteRenderer.flipX = false;
            laserModelPivot.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            weaponSpritePivot.transform.rotation = Quaternion.Euler(Vector3.zero);

        }
        else if(input.x < 0)
        {
            spriteRenderer.flipX = true;
            laserModelPivot.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            weaponSpritePivot.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }
    }

    /// <summary>
    /// Is the player on ground
    /// </summary>
    /// <returns></returns>
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, capsuleCollider.bounds.extents.y + 0.01f);
    }

    private void PlayerInput()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        FlipSprite();        
    }

    private void ShootWeapon()
    {
        // Lazer
        if(Input.GetKeyDown(KeyCode.Return))
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, Mathf.Infinity))
            {
                // Check for Idamageable
                hit.transform.GetComponent<IDamageable>()?.ReceiveDamage(1);
            }
            // Show laser model
            StartCoroutine(ShowLaser());
        }
    }

    private IEnumerator ShowLaser()
    {
        laserModelPivot.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        laserModelPivot.SetActive(false);
        yield break;
    }
}
public enum PlayerState
{ 
    idle,
    walking,
    jump,
    dead
}
