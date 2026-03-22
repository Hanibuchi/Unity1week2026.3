using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class PlayerAbilities
{
    public bool canJetDash;
    public bool canAttackDown;
    public bool canIncreaseAttack;
}

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public enum ControlPriority
    {
        Default = 0,
        Dialogue = 10,
        UI = 20,
        System = 100
    }

    public static PlayerController Instance { get; private set; }

    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 15f;

    [Header("Attacks Settings")]
    public PlayerAttack[] playerAttacks;

    [Header("Recoil Settings")]
    public float recoilForceNormal = 5f;
    public float recoilForceUp = 5f;
    public float recoilForceDown = 8f;
    public float recoilDuration = 0.15f;

    [Header("Attack Settings")]
    public float attackCooldown = 0.5f;

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

    [Header("Audio")]
    public AudioClip jumpSE;
    public AudioClip[] attackSEs;
    public AudioClip jetDashSE;

    // Components
    private Rigidbody2D rb;
    private Animator animator;

    // Input States
    private Vector2 moveInput;
    private bool isJumpButtonHeld;

    // Player States
    private bool isGrounded;
    private bool isFacingRight = true;
    private float attackCooldownTimer;

    // Jet Dash States
    private bool isJetDashing;
    private float jumpHoldTimer;
    private float jetDashTimer;
    private Vector2 jetDashDirection;
    private float defaultGravityScale;
    private bool isJetDepleted;
    private float recoilTimer;

    // Interaction
    private IInteractable currentInteractable;

    // Control flag
    private bool canControl = true;

    private class ControlRequest
    {
        public bool IsEnabled;
        public int Priority;
        public object Owner;
    }
    private List<ControlRequest> controlRequests = new List<ControlRequest>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        defaultGravityScale = rb.gravityScale;
    }

    private void Update()
    {
        if (recoilTimer > 0)
        {
            recoilTimer -= Time.deltaTime;
        }

        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        CheckGrounded();
        HandleJetDashLogic();
        UpdateAnimations();

        if (!isJetDashing && recoilTimer <= 0)
        {
            FlipController();
        }
    }

    private void FixedUpdate()
    {
        if (!canControl)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        if (isJetDashing)
        {
            rb.linearVelocity = jetDashDirection * jetDashSpeed;
        }
        else if (recoilTimer > 0)
        {
            // 反動中は入力を受け付けずに現在の速度を維持（または減速）
            // rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, 0, Time.deltaTime * 5f), rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        }
    }

    public void UpdateAbilities(PlayerAbilities newAbilities)
    {
        currentAbilities = newAbilities;

        if (currentAbilities.canIncreaseAttack && playerAttacks != null)
        {
            foreach (var attack in playerAttacks)
            {
                if (attack != null)
                {
                    attack.IncreaseAttack();
                }
            }
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (!canControl)
        {
            moveInput = Vector2.zero;
            return;
        }
        moveInput = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!canControl) return;

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
        if (!canControl) return;

        if (context.started && !isJetDashing && attackCooldownTimer <= 0)
        {
            if (SoundManager.Instance != null && attackSEs != null && attackSEs.Length > 0)
            {
                AudioClip clip = attackSEs[Random.Range(0, attackSEs.Length)];
                if (clip != null) SoundManager.Instance.PlaySE(clip);
            }

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

            attackCooldownTimer = attackCooldown;
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (!canControl) return;

        if (context.started && isGrounded && !isJetDashing)
        {
            if (currentInteractable != null)
            {
                currentInteractable.Interact();
            }
            else
            {
                Debug.Log("Interact triggered");
            }
        }
    }

    public void SetInteractable(IInteractable interactable)
    {
        currentInteractable = interactable;
    }

    public void RemoveInteractable(IInteractable interactable)
    {
        if (currentInteractable == interactable)
        {
            currentInteractable = null;
        }
    }

    /// <summary>
    /// 操作可能状態の変更を要求します。すでに同じownerからの要求がある場合は上書きされます。
    /// 優先度(priority)が最も高い要求が現在の操作状態として適用されます。
    /// </summary>
    public void SetControlEnabled(bool isEnabled, ControlPriority priority, object owner)
    {
        int priorityValue = (int)priority;
        var existingRequest = controlRequests.FirstOrDefault(r => r.Owner == owner);
        if (existingRequest != null)
        {
            existingRequest.IsEnabled = isEnabled;
            existingRequest.Priority = priorityValue;
        }
        else
        {
            controlRequests.Add(new ControlRequest { IsEnabled = isEnabled, Priority = priorityValue, Owner = owner });
        }

        ApplyHighestPriorityControl();
    }

    /// <summary>
    /// 対象ownerの操作状態変更要求を取り消します。
    /// </summary>
    public void RemoveControlRequest(object owner)
    {
        int removedCount = controlRequests.RemoveAll(r => r.Owner == owner);
        if (removedCount > 0 || controlRequests.Count == 0)
        {
            ApplyHighestPriorityControl();
        }
    }

    private void ApplyHighestPriorityControl()
    {
        if (controlRequests.Count == 0)
        {
            canControl = true;
            return;
        }

        // 最も優先度の高い要求を取得
        var highestRequest = controlRequests.OrderByDescending(r => r.Priority).First();

        bool wasControlEnabled = canControl;
        canControl = highestRequest.IsEnabled;

        if (wasControlEnabled && !canControl)
        {
            moveInput = Vector2.zero;
            isJumpButtonHeld = false;
        }
    }

    public void SetPositionAndFacing(Vector2 newPosition, bool faceRight)
    {
        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        if (isFacingRight != faceRight)
        {
            Flip();
        }
    }

    public void OnHitAttackNormal()
    {
        // Debug.Log("Hit Normal Attack");
        float recoilDir = isFacingRight ? -1f : 1f;
        rb.linearVelocity = new Vector2(recoilDir * recoilForceNormal, rb.linearVelocity.y);
        recoilTimer = recoilDuration;
    }

    public void OnHitAttackUp()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, -recoilForceUp);
        recoilTimer = recoilDuration;
    }

    public void OnHitAttackDown()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, recoilForceDown);
        recoilTimer = recoilDuration;
    }

    private void PerformJump()
    {
        if (isGrounded)
        {
            if (SoundManager.Instance != null && jumpSE != null) SoundManager.Instance.PlaySE(jumpSE);
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
    {if (SoundManager.Instance != null && jetDashSE != null) SoundManager.Instance.PlaySE(jetDashSE);
        
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

        /// <summary>
    /// 特殊アクション（ジェットダッシュなど）を強制的に解除する。
    /// </summary>
    public void ForceCancelSpecialActions()
    {
        if (isJetDashing)
        {
            StopJetDash();
        }
        // 他に今後追加される特殊アクションがあればここで解除処理を追加
    }

    public PlayerAbilities testAbilities = new PlayerAbilities
    {
        canJetDash = true,
        canAttackDown = true,
        canIncreaseAttack = true
    };
    public void TestAbilityChange()
    {
        UpdateAbilities(testAbilities);
    }
}
