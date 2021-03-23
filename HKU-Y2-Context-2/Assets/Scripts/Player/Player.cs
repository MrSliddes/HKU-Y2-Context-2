using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public bool allowedToJump = true;

    public float health = 3;
    [Header("Movement")]
    public float movementSpeed = 7f;
    /// <summary>
    /// Multiplier while walking on ground
    /// </summary>
    public float movementMultiplier = 10f;
    /// <summary>
    /// Reduces movementspeed while in air
    /// </summary>
    public float airMultiplier = 0.3f;
    /// <summary>
    /// Drag rb receives while isGrounded
    /// </summary>
    public float rbGroundDrag = 6;
    /// <summary>
    /// Drag rb receives while !isGrounded
    /// </summary>
    public float rbAirDrag = 2;
    
    [Header("Jumping")]
    /// <summary>
    /// The force used to jump
    /// </summary>
    public float jumpForce = 15;
    /// <summary>
    /// The gravity force being applied when not grounded
    /// </summary>
    public float gravityForce = 18;
    public LayerMask layerMaskGround;

    [Header("Other")]
    /// <summary>
    /// How long the player stays invincible after getting hit
    /// </summary>
    public float invincibleTime = 3f;
    public PlayerState playerState;

    /// <summary>
    /// The nockback applied to player when getting hit
    /// </summary>
    public Vector3 nockbackForce = new Vector3(5f, 3f, 0f);

    [Header("Required Components")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public Transform animatorObject;
    public GameObject spritesParent;

    [Header("Weapon")]
    public bool hasWeaponStick;
    public bool hasWeaponLaser;
    public GameObject weaponPivot;
    public Animator weaponStickAnimator;
    public GameObject prefabWeaponStickDamage;
    public GameObject laserModelPivot;

    [Header("Audio")]
    public AudioClip clipJump;
    public AudioClip clipHit;
    public AudioClip clipHurt;

    [HideInInspector] public Rigidbody rb;

    /// <summary>
    /// Current input of player
    /// </summary>
    private Vector3 input;
    /// <summary>
    /// Used for rb movement
    /// </summary>
    private Vector3 inputDirection;
    private AudioSource audioSource;

    private bool addGravityForce;
    private bool canMove;
    private bool cameOffGround;
    private bool hasEnterdNewPlayerState = false;
    private bool isGrounded;
    private bool isInvincible;
    private int currentWeapon = 0;
    private bool isFlikkerRed;

    // Start is called before the first frame update
    void Start()
    {
        // Get
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        // Set
        laserModelPivot.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, 0.9f, 0), 0.1f, layerMaskGround, QueryTriggerInteraction.Ignore); // Somehow works with trigger?

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
        audioSource.PlayOneShot(clipHurt);

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
        Color f = new Color(1, 1, 1, 100f / 255f);
        Color c = new Color(1, 1, 1, 1);
        float timeLeft = invincibleTime;
        while(timeLeft > 0)
        {
            if(!isFlikkerRed)
            {
                SpriteRenderer[] items = spritesParent.GetComponentsInChildren<SpriteRenderer>();
                foreach(SpriteRenderer item in items)
                {
                    item.color = f;
                }
            }
            else
            {
                SpriteRenderer[] items = spritesParent.GetComponentsInChildren<SpriteRenderer>();
                foreach(SpriteRenderer item in items)
                {
                    item.color = c;
                }
            }
            isFlikkerRed = !isFlikkerRed;
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
                    // Stop sliding
                    if(input.x == 0 && isGrounded) rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
                    animator.Play("player_idle");
                }

                // Update

                // Exit
                if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
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
                animator.Play("player_run");

                // Update
                PlayerInput();

                // Exit
                if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
                {
                    // Goto idle
                    EnterNewPlayerState(PlayerState.idle);
                }
                else if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
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
                    if(!allowedToJump)
                    {
                        EnterNewPlayerState(PlayerState.idle);
                        return;
                    }

                    rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                    cameOffGround = false;
                    addGravityForce = false;
                    canMove = true;
                    animator.Play("player_jump");
                    audioSource.PlayOneShot(clipJump);
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
                    animator.Play("player_death");
                    canMove = false;
                    // Show game over screen
                    Invoke("GameOver", 1);
                }

                // Update

                // Exit

                break;
            default: Debug.LogWarning("Wrong playerstate"); break;
        }
    }

    private void GameOver()
    {
        PlayerUI.ShowGameOverScreen();
    }

    private void FlipSprite()
    {
        if(input.x > 0)
        {
            spriteRenderer.flipX = false;
            laserModelPivot.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            weaponPivot.transform.rotation = Quaternion.Euler(Vector3.zero);
            animatorObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

        }
        else if(input.x < 0)
        {
            spriteRenderer.flipX = true;
            laserModelPivot.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            weaponPivot.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            animatorObject.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
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
                    if(!hasWeaponStick) return;
                    // prob should check if animation is done playing
                    weaponStickAnimator.Play("Anim_Player_Weapon_Stick_Swing");
                    Instantiate(prefabWeaponStickDamage, weaponPivot.transform.position, weaponPivot.transform.rotation);
                    animator.Play("player_hit");
                    audioSource.PlayOneShot(clipHit);
                    break;
                case 1:
                    // Laser
                    if(!hasWeaponLaser) return;
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
