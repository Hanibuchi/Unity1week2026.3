using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public abstract class EnemyController : MonoBehaviour
{
    [Header("Death Settings")]
    public float destroyDelay = 2f; // 死亡後に破棄されるまでの時間
    public Collider2D[] deathColliders; // 死亡時に無効化する複数のコライダー

    protected Rigidbody2D rb;
    protected Animator animator;

    protected bool isFacingRight = true;
    protected bool isDead = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public virtual void Die()
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

    protected virtual void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
