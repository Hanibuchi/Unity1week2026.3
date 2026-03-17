using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class PlayerAbilities
{
    public bool canJetDash;
    public bool canAttackDown;
}

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 15f;
    
    [Header("Jet Dash Settings")]
    public float jetDashSpeed = 20f;
    public float timeToTriggerJet = 0.2f;
    public float maxJetDashDuration = 1.0f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Current Abilities")]
    [SerializeField] private PlayerAbilities currentAbilities;

    // Components
    private Rigidbody2D rb;
    private Animator animator;

    // Input States
    private Vector2 moveInput;
    private bool isJumpButtonHeld;

    // Player States
    private bool isGrounded;
    private bool isFacingRight = true;

    // Jet Dash States
    private bool isJetDashing;
    private float jumpHoldTimer;
    private float jetDashTimer;
    private Vector2 jetDashDirection;
    private float defaultGravityScale;
    private bool isJetDepleted;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        defaultGravityScale = rb.gravityScale;
    }

    private void Update()
    {
        CheckGrounded();
        HandleJetDashLogic();
        UpdateAnimations();

        if (!isJetDashing)
        {
            FlipController();
        }
    }

    private void FixedUpdate()
    {
        if (isJetDashing)
        {
            rb.linearVelocity = jetDashDirection * jetDashSpeed;
        }
        else
        {
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        }
    }

    public void UpdateAbilities(PlayerAbilities newAbilities)
    {
        currentAbilities = newAbilities;
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isJumpButtonHeld = true;
            jumpHoldTimer = 0f;
            PerformJump();
        }
        else if (context.canceled)
        {
            isJumpButtonHeld = false;
            
            if (isJetDashing)
            {
                StopJetDash();
            }
            else if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            }
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.started && !isJetDashing)
        {
            if (moveInput.y > 0.5f)
            {
                animator.SetTrigger("OnAttackUp");
            }
            else if (moveInput.y < -0.5f && !isGrounded && currentAbilities.canAttackDown)
            {
                animator.SetTrigger("OnAttackDown");
            }
            else
            {
                animator.SetTrigger("OnAttackNormal");
            }
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded && !isJetDashing)
        {
            Debug.Log("Interact triggered");
        }
    }

    private void PerformJump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void HandleJetDashLogic()
    {
        if (!currentAbilities.canJetDash) return;

        if (isJumpButtonHeld)
        {
            jumpHoldTimer += Time.deltaTime;

            if (!isJetDashing && jumpHoldTimer >= timeToTriggerJet && moveInput.magnitude > 0.1f && !isJetDepleted)
            {
                StartJetDash();
            }
        }

        if (isJetDashing)
        {
            if (moveInput.magnitude > 0.1f)
            {
                jetDashDirection = moveInput.normalized;
            }

            jetDashTimer -= Time.deltaTime;
            if (jetDashTimer <= 0)
            {
                isJetDepleted = true;
                StopJetDash();
            }
        }
    }

    private void StartJetDash()
    {
        isJetDashing = true;
        jetDashDirection = moveInput.normalized;
        rb.gravityScale = 0f;
    }

    private void StopJetDash()
    {
        if (!isJetDashing) return;

        isJetDashing = false;
        rb.gravityScale = defaultGravityScale;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.25f);
    }

    private void CheckGrounded()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        if (isGrounded)
        {
            jetDashTimer = maxJetDashDuration;
            isJetDepleted = false;
        }
    }

    private void FlipController()
    {
        if (moveInput.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveInput.x < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void UpdateAnimations()
    {
        if (animator == null) return;
        
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsMoving", Mathf.Abs(moveInput.x) > 0.1f);

        animator.SetBool("IsJetting", isJetDashing);
        if (isJetDashing)
        {
            float jetDirValue = 1f; // 横 (Horizontal)
            if (jetDashDirection.y > 0.5f)
            {
                jetDirValue = 0f; // 上 (Up)
            }
            else if (jetDashDirection.y < -0.5f)
            {
                jetDirValue = 2f; // 下 (Down)
            }
            animator.SetFloat("JetDirection", jetDirValue);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
