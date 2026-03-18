using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 10;
    public int currentHealth;

    [Header("Audio")]
    public AudioClip damageSE;
    public AudioClip dieSE;

    [Header("Fragment Settings")]
    public string fragmentPrefabPath = "Fragment";
    [Range(0f, 1f)]
    public float fragmentSpawnProbability = 0.5f;
    public float minFragmentForce = 5f;
    public float maxFragmentForce = 15f;
    public float fragmentLifetime = 2f;

    [Header("Hit Stop Settings")]
    public float hitStopDuration = 0.05f;
    [Range(0f, 1f)]
    public float hitStopTimeScale = 0.05f;
    public TimeScaleManager.TimeScalePriority hitStopPriority = TimeScaleManager.TimeScalePriority.HitStop;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // プレイヤーの攻撃を検出
        PlayerAttack playerAttack = other.GetComponent<PlayerAttack>();
        if (playerAttack != null)
        {
            // ダメージ処理（必要に応じて変更してください）
            TakeDamage(playerAttack.damage);

            // プレイヤーの攻撃用クラスに通知
            playerAttack.OnHitTarget(this);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (SoundManager.Instance != null && damageSE != null) SoundManager.Instance.PlaySE(damageSE);

        SpawnFragments();

        if (hitStopDuration > 0f)
        {
            StartCoroutine(HitStopRoutine());
        }

        EnemyController enemyController = GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.OnDamage();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private System.Collections.IEnumerator HitStopRoutine()
    {
        if (TimeScaleManager.Instance != null)
        {
            TimeScaleManager.Instance.SetTimeScale(hitStopTimeScale, (int)hitStopPriority, this);
            yield return new WaitForSecondsRealtime(hitStopDuration);
            TimeScaleManager.Instance.RemoveRequest(this);
        }
    }

    private void SpawnFragments()
    {
        GameObject prefab = Resources.Load<GameObject>(fragmentPrefabPath);
        if (prefab == null) return;

        while (Random.value < fragmentSpawnProbability)
        {
            // ランダムな角度で
            Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
            GameObject fragment = Instantiate(prefab, transform.position, randomRotation);
            
            // 一定時間後に削除
            Destroy(fragment, fragmentLifetime);
            
            Rigidbody2D rb = fragment.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // ランダムな方向に
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                // ランダムな強さで
                float randomForce = Random.Range(minFragmentForce, maxFragmentForce);
                
                rb.AddForce(randomDirection * randomForce, ForceMode2D.Impulse);
                rb.AddTorque(Random.Range(-randomForce, randomForce), ForceMode2D.Impulse);
            }
        }
    }

    private void Die()
    {
        if (SoundManager.Instance != null && dieSE != null) SoundManager.Instance.PlaySE(dieSE);

        // EnemyControllerがあればアニメーション等の死亡処理を任せる
        EnemyController enemyController = GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.Die();
        }
        else
        {
            // なければ即座に破棄
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (TimeScaleManager.Instance != null)
        {
            TimeScaleManager.Instance.RemoveRequest(this);
        }
    }
}
