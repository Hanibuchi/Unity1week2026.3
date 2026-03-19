using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }

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
    }

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private int currentHealth;

    [Header("Invincibility Settings")]
    [SerializeField] private float invincibilityDuration = 1.0f;
    [SerializeField] private Animator animator;
    [SerializeField] private string invincibleAnimParam = "IsInvincible";

    [Header("Item Collection Settings")]
    [SerializeField] private LayerMask healItemLayer;
    [SerializeField] private float pullRadius = 5.0f;
    [SerializeField] private float pullSpeed = 10.0f;
    [SerializeField] private int itemsNeededToHeal = 5;
    [SerializeField] private int currentItemCount = 0;

    private bool isInvincible;
    private float invincibilityTimer;

    [Header("Camera Feedback")]
    [SerializeField] private float damageShakeForce = 0.2f;

    [Header("Audio")]
    [SerializeField] private AudioClip damageSE;
    [SerializeField] private AudioClip healItemSE;
    [SerializeField] private AudioClip healSE;
    [SerializeField] private AudioClip dieSE;

    [Header("Events")]
    public UnityEvent<int, int> OnHealthChanged; // Current HP, Max HP
    public UnityEvent<int, int> OnHealItemProgressChanged; // Current Item Count, Required Items
    public UnityEvent OnTakeDamage;
    public UnityEvent OnDie;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    private void Start()
    {
        InitializeHealth();
        if (animator == null) animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                SetInvincibility(false);
            }
        }

        PullHealItems();
    }

    private void PullHealItems()
    {
        if (healItemLayer.value == 0) return;

        Collider2D[] itemsInRange = Physics2D.OverlapCircleAll(transform.position, pullRadius, healItemLayer);
        foreach (var item in itemsInRange)
        {
            if (item.TryGetComponent<Rigidbody2D>(out var itemRb))
            {
                // 対象に向かう方向ベクトル
                Vector2 direction = (transform.position - item.transform.position).normalized;
                // Rigidbody2Dを使って力を加える
                itemRb.AddForce(direction * pullSpeed * Time.deltaTime * 50f, ForceMode2D.Force);
            }
            else
            {
                // Rigidbody2Dがない場合の処理（元の挙動）
                item.transform.position = Vector3.MoveTowards(item.transform.position, transform.position, pullSpeed * Time.deltaTime);
            }
        }
    }

    private void SetInvincibility(bool state)
    {
        isInvincible = state;
        if (state)
        {
            invincibilityTimer = invincibilityDuration;
        }

        if (animator != null)
        {
            animator.SetBool(invincibleAnimParam, state);
        }
    }

    public void InitializeHealth()
    {
        currentHealth = maxHealth;
        currentItemCount = 0;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnHealItemProgressChanged?.Invoke(currentItemCount, itemsNeededToHeal);
    }

    public void TakeDamage(int damageAmount)
    {
        if (currentHealth <= 0 || damageAmount <= 0 || isInvincible) return;

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (animator != null)
        {
            animator.SetTrigger("Damage");
        }

        SetInvincibility(true);

        if (SoundManager.Instance != null && damageSE != null) SoundManager.Instance.PlaySE(damageSE);

        if (CameraController.Instance != null)
        {
            CameraController.Instance.ShakeScreen(damageShakeForce);
        }

        OnTakeDamage?.Invoke();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // ダメージを受けた際、貯めていたアイテムがあれば消費して回復する
            TryConsumeHealItems();
        }
    }

    public void Heal(int healAmount)
    {
        if (currentHealth <= 0 || healAmount <= 0) return;

        currentHealth += healAmount;
        if (SoundManager.Instance != null && healSE != null) SoundManager.Instance.PlaySE(healSE);

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        if (SoundManager.Instance != null && dieSE != null) SoundManager.Instance.PlaySE(dieSE);
        OnDie?.Invoke();
        // 死亡時の処理はOnDieイベントに登録して外部で制御できるようにしています。
        // 必要に応じてここに直接記述しても問題ありません。
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<EnemyAttack>(out var enemyAttack))
        {
            TakeDamage(enemyAttack.DamageAmount);
        }

        // アイテムのレイヤーかチェック
        if (((1 << collision.gameObject.layer) & healItemLayer.value) != 0)
        {
            CollectHealItem(collision.gameObject);
        }
    }

    private void CollectHealItem(GameObject itemObj)
    {
        Destroy(itemObj); // アイテムを削除
        if (SoundManager.Instance != null && healItemSE != null) SoundManager.Instance.PlaySE(healItemSE);


        // 体力が最大の時に、規定数を超えてアイテムをストックしないように制限
        if (currentHealth >= maxHealth && currentItemCount >= itemsNeededToHeal)
        {
            // アイテムは吸収（削除）されるが、カウントは増やさない
        }
        else
        {
            currentItemCount++;
            OnHealItemProgressChanged?.Invoke(currentItemCount, itemsNeededToHeal);
        }

        TryConsumeHealItems();
    }

    private void TryConsumeHealItems()
    {
        // 体力が最大値よりも低く、アイテムが規定数集まっていれば消費して継続的に回復する
        while (currentHealth < maxHealth && currentItemCount >= itemsNeededToHeal)
        {
            currentItemCount -= itemsNeededToHeal;
            Heal(1);
            OnHealItemProgressChanged?.Invoke(currentItemCount, itemsNeededToHeal);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<EnemyAttack>(out var enemyAttack))
        {
            TakeDamage(enemyAttack.DamageAmount);
        }

        if (((1 << collision.gameObject.layer) & healItemLayer.value) != 0)
        {
            CollectHealItem(collision.gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pullRadius);
    }
}