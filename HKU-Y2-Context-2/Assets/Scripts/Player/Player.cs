using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float health = 3;
    public float movementSpeed = 5f;

    public float jumpForce;
    public float jumpVelTillGravity = 8;
    public float gravityForce = 1;
    public float maxGravityVelocity = -20;

    public float invincibleTime = 3f;
    public PlayerState playerState;

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
    private bool cameOffGround;
    private bool hasEnterdNewPlayerState = false;
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
        PlayerInput();
        UpdatePlayerState();

        UseWeapon();

        inputDirection = new Vector3(input.x * movementSpeed, rb.velocity.y, rb.velocity.z);
    }

    private void FixedUpdate()
    {
        // Movement
        rb.velocity = inputDirection;

        // Gravity
        if(addGravityForce || !IsGrounded() && playerState != PlayerState.jump)
        {
            rb.AddForce(Vector3.down * gravityForce * 1000, ForceMode.Force);
        }
        // Limit gravity
        if(rb.velocity.y < maxGravityVelocity) rb.velocity = new Vector3(rb.velocity.x, maxGravityVelocity, rb.velocity.z);
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
                if(Input.GetKeyDown(KeyCode.Space) && IsGrounded())
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
                    rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
                    cameOffGround = false;
                    addGravityForce = false;
                }

                // Update
                if(!IsGrounded()) cameOffGround = true;
                // Check for negative velocity, that means the player is coming down
                addGravityForce = rb.velocity.y < jumpVelTillGravity;

                // Exit
                if(cameOffGround && IsGrounded())
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

    /// <summary>
    /// Is the player on ground
    /// </summary>
    /// <returns></returns>
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.01f);
    }

    private void PlayerInput()
    {
        input.x = Input.GetAxisRaw("Horizontal");
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
}
public enum PlayerState
{ 
    idle,
    walking,
    jump,
    dead
}
