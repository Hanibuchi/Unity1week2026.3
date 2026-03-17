using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private int currentHealth;

    [Header("Invincibility Settings")]
    [SerializeField] private float invincibilityDuration = 1.0f;
    [SerializeField] private Animator animator;
    [SerializeField] private string invincibleAnimParam = "IsInvincible";

    private bool isInvincible;
    private float invincibilityTimer;

    [Header("Events")]
    public UnityEvent<int, int> OnHealthChanged; // Current HP, Max HP
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
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damageAmount)
    {
        if (currentHealth <= 0 || damageAmount <= 0 || isInvincible) return;

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        SetInvincibility(true);

        OnTakeDamage?.Invoke();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        if (currentHealth <= 0 || healAmount <= 0) return;

        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<EnemyAttack>(out var enemyAttack))
        {
            TakeDamage(enemyAttack.DamageAmount);
        }
    }
}