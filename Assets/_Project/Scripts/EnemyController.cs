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

        // Resourcesフォルダー内から"HealItem"이라는名前のプレハブを取得（必要に応じてパスを変更してください）
        GameObject healItemPrefab = Resources.Load<GameObject>("HealItem");
        if (healItemPrefab == null)
        {
            Debug.LogWarning("HealItem prefab not found in Resources folder.");
            return;
        }

        for (int i = 0; i < dropItemCount; i++)
        {
            // アイテムを生成
            GameObject item = Instantiate(healItemPrefab, transform.position, Quaternion.identity);

            // Rigidbody2Dがあれば散らばらせる
            Rigidbody2D itemRb = item.GetComponent<Rigidbody2D>();
            if (itemRb != null)
            {
                // ランダムな方向（上方向が強め）を計算
                Vector2 randomDir = Random.insideUnitCircle.normalized;
                randomDir.y = Mathf.Abs(randomDir.y) + 0.5f; // 上方向に補正
                randomDir.Normalize();

                itemRb.AddForce(randomDir * dropScatterForce, ForceMode2D.Impulse);
            }
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
