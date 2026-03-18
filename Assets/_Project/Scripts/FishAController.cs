using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class FishAController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float moveInterval = 2f; // 移動と移動の間の待機時間
    public float moveDuration = 1f; // 1回の移動にかける時間

    [Header("Death Settings")]
    public float destroyDelay = 2f; // 死亡後に破棄されるまでの時間
    public Collider2D[] deathColliders; // 死亡時に無効化する複数のコライダー

    private Rigidbody2D rb;
    private Animator animator;

    private float actionTimer = 0f;
    private bool isMovingState = false;
    private bool isFacingRight = true;
    private bool isDead = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        // 無重力として扱うため重力を0にする
        rb.gravityScale = 0f;
        
        // 最初の最初は待機状態から始める
        actionTimer = moveInterval;
    }

    private void Update()
    {
        if (isDead) return;

        actionTimer -= Time.deltaTime;

        if (isMovingState)
        {
            // 移動時間の終了判定
            if (actionTimer <= 0f)
            {
                StopMoving();
            }
        }
        else
        {
            // 待機時間の終了判定
            if (actionTimer <= 0f)
            {
                StartMoving();
            }
        }
    }

    private void StartMoving()
    {
        isMovingState = true;
        actionTimer = moveDuration;

        // ランダムな方向を決定
        Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        rb.linearVelocity = randomDirection * moveSpeed;

        animator.SetBool("IsMoving", true);

        // X成分の向きに合わせて反転
        if (randomDirection.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (randomDirection.x < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void StopMoving()
    {
        isMovingState = false;
        actionTimer = moveInterval;
        
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("IsMoving", false);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        // 死亡アニメーション（Trigger）を再生
        animator.SetTrigger("Die");
        
        // 移動を停止
        rb.linearVelocity = Vector2.zero;
        
        // 必要に応じて当たり判定や物理演算を無効化
        if (deathColliders != null)
        {
            foreach (var col in deathColliders)
            {
                if (col != null)
                {
                    col.enabled = false;
                }
            }
        }
        rb.bodyType = RigidbodyType2D.Kinematic;

        // 一定時間後にオブジェクトを破棄
        Destroy(gameObject, destroyDelay);
    }
}
