using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float health = 3;
    [Header("Movement")]
    public float movementSpeed = 5f;
    public float movementMultiplier = 10f;
    /// <summary>
    /// Reduces movementspeed while in air
    /// </summary>
    public float airMultiplier = 0.4f;
    public float rbGroundDrag = 6;
    public float rbAirDrag = 2;
    
    [Header("Jumping")]
    public float jumpForce = 5;
    public float gravityForce = 1;
    public LayerMask layerMaskGround;

    [Header("Other")]
    public float invincibleTime = 3f;
    public PlayerState playerState;

    public Vector3 nockbackForce = new Vector3(1f, 1f, 0f);

    [Header("Required Components")]
    public SpriteRenderer spriteRenderer;

    [Header("Weapon")]
    public GameObject weaponPivot;
    public Animator weaponStickAnimator;
    public GameObject prefabWeaponStickDamage;
    public GameObject laserModelPivot;

    private bool isInvincible;
    [HideInInspector] public Rigidbody rb;
    private CapsuleCollider capsuleCollider;

    /// <summary>
    /// Current input of player
    /// </summary>
    private Vector3 input;
    /// <summary>
    /// Used for rb movement
    /// </summary>
    private Vector3 inputDirection;

    private bool addGravityForce;
    private bool canMove;
    private bool cameOffGround;
    private bool hasEnterdNewPlayerState = false;
    private bool isGrounded;
    private int currentWeapon = 0;

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
        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, 0.9f, 0), 0.1f, layerMaskGround);

        PlayerInput();
        UpdatePlayerState();

        UseWeapon();
                
        ControlDrag();
    }

    private void FixedUpdate()
    {
        // Movement => https://www.youtube.com/watch?v=E5zNi_SSP_w
        if(canMove)
        {
            if(isGrounded)
            {
                rb.AddForce(inputDirection * movementSpeed * movementMultiplier, ForceMode.Acceleration);
            }
            else
            {
                rb.AddForce(inputDirection * movementSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
            }
        }

        if(!isGrounded && rb.velocity.y < 0)
        {
            // Add gravity when player is falling down
            rb.AddForce(Vector3.down * gravityForce, ForceMode.Acceleration);
        }
        else if(!isGrounded && rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            // Add gravity when player is not holding space anymore
            rb.AddForce(Vector3.down * gravityForce, ForceMode.Acceleration);
        }
        //// Gravity
        //if(addGravityForce || !IsGrounded() && playerState != PlayerState.jump)
        //{
        //    rb.AddForce(Vector3.down * gravityForce * 1000, ForceMode.Force);
        //}
        //// Limit gravity
        //if(rb.velocity.y < maxGravityVelocity) rb.velocity = new Vector3(rb.velocity.x, maxGravityVelocity, rb.velocity.z);
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
    public void ReceiveDamage(float damage, Transform damageDealer) // Not triggerd by IDamageable since we dont want the player to kill himself
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

        // Based on damageDealer position add knockback
        if(damageDealer.transform.position.x > transform.position.x)
        {
            // Nock left
            rb.AddForce(Vector3.left * nockbackForce.x + Vector3.up * nockbackForce.y, ForceMode.VelocityChange);
        }
        else
        {
            // Nock right
            rb.AddForce(Vector3.right * nockbackForce.x + Vector3.up * nockbackForce.y, ForceMode.VelocityChange);
        }
    }

    private IEnumerator InvisTimeAsync()
    {
        isInvincible = true;
        // Sprite flikker
        Color f = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 100f / 255f);
        Color c = spriteRenderer.color;
        spriteRenderer.color = f;
        float timeLeft = invincibleTime;
        while(timeLeft > 0)
        {
            if(spriteRenderer.color == c) spriteRenderer.color = f; else spriteRenderer.color = c;
            timeLeft -= 0.15f;
            yield return new WaitForSeconds(0.15f);
        }

        spriteRenderer.color = c;
        isInvincible = false;
        yield break;
    }

    private void ControlDrag()
    {
        if(isGrounded)
        {
            rb.drag = rbGroundDrag;
        }
        else
        {
            rb.drag = rbAirDrag;
        }
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
                    canMove = true;
                }

                // Update

                // Exit
                if(Input.GetKey(KeyCode.Space) && isGrounded)
                {
                    // Goto jump
                    EnterNewPlayerState(PlayerState.jump);
                }
                else if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                {
                    // Goto walking
                    EnterNewPlayerState(PlayerState.walking);
                }
                break;
            case PlayerState.walking:
                // Enter
                if(!hasEnterdNewPlayerState)
                {
                    hasEnterdNewPlayerState = true;
                    canMove = true;
                }

                // Update
                PlayerInput();

                // Exit
                if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
                {
                    // Goto idle
                    EnterNewPlayerState(PlayerState.idle);
                }
                else if(Input.GetKey(KeyCode.Space) && isGrounded)
                {
                    // Goto jump
                    EnterNewPlayerState(PlayerState.jump);
                }
                break;
            case PlayerState.jump:
                // Enter
                if(!hasEnterdNewPlayerState)
                {
                    hasEnterdNewPlayerState = true;
                    rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                    cameOffGround = false;
                    addGravityForce = false;
                    canMove = true;
                }

                // Update
                // Fall down
                if(!isGrounded) cameOffGround = true;

                // Exit
                if(cameOffGround && isGrounded)
                {
                    addGravityForce = false;
                    EnterNewPlayerState(PlayerState.idle);
                }
                break;
            case PlayerState.dead:
                // Enter
                if(!hasEnterdNewPlayerState)
                {
                    hasEnterdNewPlayerState = true;
                    // Restart level
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
            weaponPivot.transform.rotation = Quaternion.Euler(Vector3.zero);

        }
        else if(input.x < 0)
        {
            spriteRenderer.flipX = true;
            laserModelPivot.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            weaponPivot.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }
    }

    

    private void PlayerInput()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        inputDirection = new Vector3(input.x, 0, 0);
        FlipSprite();        
    }

    private void UseWeapon()
    {
        // Lazer
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            switch(currentWeapon)
            {
                case 0:
                    // Stick
                    // prob should check if animation is done playing
                    weaponStickAnimator.Play("Anim_Player_Weapon_Stick_Swing");
                    Instantiate(prefabWeaponStickDamage, weaponPivot.transform.position, weaponPivot.transform.rotation);
                    break;
                case 1:
                    // Laser
                    RaycastHit hit;
                    if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, Mathf.Infinity))
                    {
                        // Check for Idamageable
                        hit.transform.GetComponent<IDamageable>()?.ReceiveDamage(1);
                    }
                    // Show laser model
                    StartCoroutine(ShowLaser());
                    break;
                default:Debug.LogError("Unknown weapon" + currentWeapon); break;
            }

        }
    }

    private IEnumerator ShowLaser()
    {
        laserModelPivot.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        laserModelPivot.SetActive(false);
        yield break;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position - new Vector3(0, 0.9f, 0), 0.1f);
    }
}
public enum PlayerState
{ 
    idle,
    walking,
    jump,
    dead
}
