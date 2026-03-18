using UnityEngine;

public class FishAController : EnemyController
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float moveInterval = 2f; // 移動と移動の間の待機時間
    public float moveDuration = 1f; // 1回の移動にかける時間

    private float actionTimer = 0f;
    private bool isMovingState = false;

    protected override void Awake()
    {
        base.Awake();
        
        // 無重力として扱うため重力を0にする
        rb.gravityScale = 0f;
        
        // 最初の最初は待機状態から始める
        actionTimer = moveInterval;
    }

    protected virtual void Update()
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
}
