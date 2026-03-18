using UnityEngine;

public class ChildController : EnemyController
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;

    [Header("Attack Settings")]
    public Transform attackSensor;
    public float attackRange = 1f;
    public LayerMask playerLayer;
    public float minAttackWaitTime = 1f; // 攻撃後、その場に留まる時間の最小値
    public float maxAttackWaitTime = 3f; // 攻撃後、その場に留まる時間の最大値

    [Header("Detection Settings")]
    public float detectionRadius = 10f; // プレイヤーを検知する半径

    [Header("Audio")]
    public AudioClip attackSE;
    public float attackScreenShakeForce = 0f;

    private Transform player;

    private float waitTimer = 0f;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        if (isDead) return;

        FindPlayerInRange();

        if (player == null)
        {
            // プレイヤーが範囲内にいない場合は停止
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetBool("IsMoving", false);
            return;
        }

        // 待機時間（攻撃後の停止時間）の処理
        if (waitTimer > 0)
        {
            waitTimer -= Time.deltaTime;
            
            // 待機中は動かないようにする
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetBool("IsMoving", false);
            return;
        }

        // 攻撃範囲のチェック
        CheckAttack();

        // 攻撃待機状態になっていなければ移動する
        if (waitTimer <= 0)
        {
            MoveTowardsPlayer();
        }
    }

    private void CheckAttack()
    {
        if (attackSensor == null) return;

        // センサーの位置から円形に判定を行い、プレイヤーがいるか確認
        Collider2D hit = Physics2D.OverlapCircle(attackSensor.position, attackRange, playerLayer);
        if (hit != null && hit.CompareTag("Player"))
        {
            // プレイヤーが範囲内にいれば攻撃
            animator.SetTrigger("Attack");
            
            // 一定時間その場に留まるためのタイマーを設定
            waitTimer = Random.Range(minAttackWaitTime, maxAttackWaitTime);
            
            // その場で停止
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetBool("IsMoving", false);
        }
    }

    // アニメーションイベントから呼び出す用
    public void PlayAttackSE()
    {
        if (SoundManager.Instance != null && attackSE != null)
        {
            SoundManager.Instance.PlaySE(attackSE);
        }

        if (attackScreenShakeForce > 0f && CameraController.Instance != null)
        {
            CameraController.Instance.ShakeScreen(attackScreenShakeForce);
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

    private void MoveTowardsPlayer()
    {
        // プレイヤーの方向を計算
        float distanceX = player.position.x - transform.position.x;
        float direction = Mathf.Sign(distanceX);
        
        // プレイヤーとほとんど同じX座標にいる場合は無理に動かさない
        if (Mathf.Abs(distanceX) < 0.1f)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetBool("IsMoving", false);
            return;
        }

        // 移動
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
        animator.SetBool("IsMoving", true);

        // 向きの反転処理
        if (direction > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (direction < 0 && isFacingRight)
        {
            Flip();
        }
    }

    // 攻撃センサーと検知範囲の範囲をエディタ上で視覚化する
    private void OnDrawGizmosSelected()
    {
        if (attackSensor != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackSensor.position, attackRange);
        }

        // プレイヤー検知範囲
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
