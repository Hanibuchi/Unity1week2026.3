using UnityEngine;

public class FishCController : EnemyController
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float moveInterval = 2f; // 移動と移動の間の待機時間
    public float moveDuration = 1f; // 1回の移動にかける時間

    [Header("Detection Settings")]
    public float detectionRadius = 10f; // プレイヤーを検知する半径

    [Header("Attack Settings")]
    public Transform attackSensor;
    public float attackRange = 1f;
    public LayerMask playerLayer;
    public float attackWaitTime = 2f; // 攻撃後、その場に留まる時間

    [Header("Audio")]
    public AudioClip attackSE;

    private float actionTimer = 0f;
    private bool isMovingState = false;
    private float waitTimer = 0f;
    
    private Transform player;

    protected override void Awake()
    {
        base.Awake();
        
        // 無重力として扱うため重力を0にする
        rb.gravityScale = 0f;
        
        // 最初の最初は待機状態から始める
        actionTimer = moveInterval;
    }

    private void Update()
    {
        if (isDead) return;

        // 待機時間（攻撃後の停止時間）の処理
        if (waitTimer > 0)
        {
            waitTimer -= Time.deltaTime;
            
            // 待機中は動かないようにする
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsMoving", false);
            return;
        }

        // プレイヤーの検知
        FindPlayerInRange();

        // 攻撃範囲のチェック
        if (CheckAttack())
        {
            return; // 攻撃を行った場合は以後の移動をスキップ
        }

        // 基本的な移動/待機サイクルの更新
        actionTimer -= Time.deltaTime;

        if (isMovingState)
        {
            // 移動時間の終了判定
            if (actionTimer <= 0f)
            {
                StopMoving();
            }
            else if (player != null)
            {
                // 移動中かつプレイヤーが範囲内にいる場合、随時プレイヤーの方向へ軌道修正する
                Vector2 targetDirection = (player.position - transform.position).normalized;
                rb.linearVelocity = targetDirection * moveSpeed;
                UpdateFacing(targetDirection.x);
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

        Vector2 direction;

        if (player != null)
        {
            // プレイヤーが検知範囲にいれば、プレイヤーの方向へ
            direction = (player.position - transform.position).normalized;
        }
        else
        {
            // プレイヤーがいなければ、ランダムな方向を決定
            direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        }

        rb.linearVelocity = direction * moveSpeed;
        animator.SetBool("IsMoving", true);

        // X成分の向きに合わせて反転
        UpdateFacing(direction.x);
    }

    private void StopMoving()
    {
        isMovingState = false;
        actionTimer = moveInterval;
        
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("IsMoving", false);
    }

    private void UpdateFacing(float xDirection)
    {
        if (xDirection > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (xDirection < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void FindPlayerInRange()
    {
        player = null; // 一旦リセット
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, playerLayer);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                player = hit.transform;
                break;
            }
        }
    }

    private bool CheckAttack()
    {
        // センサーが未設定の場合は自身の位置を基準にする
        Vector2 checkPos = attackSensor != null ? attackSensor.position : transform.position;

        // センサーの位置から円形に判定を行い、プレイヤーがいるか確認
        Collider2D hit = Physics2D.OverlapCircle(checkPos, attackRange, playerLayer);
        if (hit != null && hit.CompareTag("Player"))
        {
            // プレイヤーが範囲内にいれば攻撃
            animator.SetTrigger("Attack");
            
            // 一定時間その場に留まるためのタイマーを設定
            waitTimer = attackWaitTime;
            
            // その場で停止
            StopMoving();
            
            return true;
        }
        
        return false;
    }

    // アニメーションイベントから呼び出す用
    public void PlayAttackSE()
    {
        if (SoundManager.Instance != null && attackSE != null)
        {
            SoundManager.Instance.PlaySE(attackSE);
        }
    }

    // 攻撃センサーと検知範囲の範囲をエディタ上で視覚化する
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 drawPos = attackSensor != null ? attackSensor.position : transform.position;
        Gizmos.DrawWireSphere(drawPos, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
