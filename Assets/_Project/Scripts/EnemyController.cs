using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public abstract class EnemyController : MonoBehaviour
{
    [Header("Death Settings")]
    public float destroyDelay = 2f; // 死亡後に破棄されるまでの時間
    public Collider2D[] deathColliders; // 死亡時に無効化する複数のコライダー

    [Header("Drop Settings")]
    public int dropItemCount = 3; // ドロップするアイテムの数
    public float dropScatterForce = 5f; // アイテムが散らばる力

    protected Rigidbody2D rb;
    protected Animator animator;

    protected bool isFacingRight = true;
    protected bool isDead = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public virtual void OnDamage()
    {
        if (isDead) return;
        animator.SetTrigger("Damage");
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

        // アイテムをドロップ
        DropItems();

        // 一定時間後にオブジェクトを破棄
        Destroy(gameObject, destroyDelay);
    }

    protected virtual void DropItems()
    {
        if (dropItemCount <= 0) return;

        // ドロップ管理のシングルトンを利用して0.1秒間隔でドロップさせる
        if (HealItemDropManager.Instance != null)
        {
            HealItemDropManager.Instance.DropItems(transform.position, dropItemCount, dropScatterForce);
        }
        else
        {
            Debug.LogWarning("HealItemDropManager object could not be found. Please add it to your scene.");
        }
    }

    protected virtual void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
