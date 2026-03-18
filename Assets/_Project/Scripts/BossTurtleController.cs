using UnityEngine;

public class BossTurtleController : EnemyController
{
    private enum BossState
    {
        ChooseAction,
        HorizontalMove,
        AttackPattern
    }

    [Header("水平移動設定 (Horizontal Move)")]
    public float horizontalMoveSpeed = 4f;
    public float verticalMoveSpeed = 2f;
    public float minHeight = -3f;
    public float maxHeight = 3f;
    public float fixedHorizontalHeight = 0f; // 水平移動を行う時の固定Y座標
    public float[] startXPositions = new float[] { -10f, 10f }; // 水平移動の開始X座標（複数設定可）
    public float horizontalMoveDuration = 5f; // 水平移動を行う時間

    [Header("攻撃設定 (Attack Pattern)")]
    public Transform attackSensor;
    public float attackRange = 1.5f;
    public LayerMask playerLayer;
    public float chaseSpeed = 3f;
    public float maxAttackStateTime = 10f; // 攻撃パターンが終了するまでの最大時間
    public float minAttackCooldown = 1f; // 攻撃同士のインターバルの最小値
    public float maxAttackCooldown = 2f; // 攻撃同士のインターバルの最大値
    public float detectionRadius = 15f;

    [SerializeField] private BossState currentState = BossState.ChooseAction;
    private float stateTimer = 0f;

    // 攻撃パターン用変数
    private int currentAttackCount = 0;
    private float attackCooldownTimer = 0f;
    private Transform player;

    // 水平移動用変数
    private float targetHeight;
    private float targetHorizontalX;
    private int horizontalDirection = 1;

    protected override void Awake()
    {
        base.Awake();
        // 壁を貫通、重力の影響を受けないようにする
        rb.gravityScale = 0f;
        // 初期状態をランダムに決定するためにタイマーを少しセットするか、直接遷移させる
        stateTimer = .1f;
    }

    private void Update()
    {
        if (isDead) return;

        FindPlayerInRange();

        switch (currentState)
        {
            case BossState.ChooseAction:
                UpdateChooseAction();
                break;
            case BossState.HorizontalMove:
                UpdateHorizontalMove();
                break;
            case BossState.AttackPattern:
                UpdateAttackPattern();
                break;
        }
    }

    private void UpdateChooseAction()
    {
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            // 次の行動をランダムに決定
            int nextAction = Random.Range(0, 2);

            // 高さを計算（水平移動なら固定値、それ以外ならランダムな範囲）
            float startY = nextAction == 0 ? fixedHorizontalHeight : Random.Range(minHeight, maxHeight);

            // 共通: どちらの行動に遷移する場合でも、指定されたX座標の端へワープする
            if (startXPositions != null && startXPositions.Length > 0)
            {
                float startX = startXPositions[Random.Range(0, startXPositions.Length)];
                transform.position = new Vector3(startX, startY, transform.position.z);

                // 開始位置から画面中央へ向かうように初期の方向を設定
                horizontalDirection = startX < 0 ? 1 : -1;
            }
            else
            {
                // 左右どちらかランダムな方向へ
                horizontalDirection = Random.Range(0, 2) == 0 ? 1 : -1;
                transform.position = new Vector3(transform.position.x, startY, transform.position.z);
            }

            // 出現時に中央の方向を向く
            if ((horizontalDirection > 0 && !isFacingRight) || (horizontalDirection < 0 && isFacingRight))
            {
                Flip();
            }

            if (nextAction == 0)
            {
                // 1. 水平移動の準備
                currentState = BossState.HorizontalMove;
                stateTimer = horizontalMoveDuration; // 保険としてのタイマー
                targetHeight = startY; // 水平移動中はこの高さを基準・目標にする

                // 向かう先のX座標（一番遠い startXPositions）を設定
                targetHorizontalX = transform.position.x;
                if (startXPositions != null && startXPositions.Length > 0)
                {
                    float maxDist = -1f;
                    foreach (float x in startXPositions)
                    {
                        float dist = Mathf.Abs(x - transform.position.x);
                        if (dist > maxDist)
                        {
                            maxDist = dist;
                            targetHorizontalX = x;
                        }
                    }
                }
            }
            else
            {
                // 2. 攻撃パターンの準備
                currentState = BossState.AttackPattern;
                stateTimer = maxAttackStateTime;
                currentAttackCount = 0;
                attackCooldownTimer = 0f;
            }
        }
    }

    private void UpdateHorizontalMove()
    {
        stateTimer -= Time.deltaTime;

        // 向かっている方向の端（ターゲットX座標）を超えたか判定
        bool reachedTarget = false;
        if (horizontalDirection > 0 && transform.position.x >= targetHorizontalX) reachedTarget = true;
        if (horizontalDirection < 0 && transform.position.x <= targetHorizontalX) reachedTarget = true;

        if (stateTimer <= 0f || reachedTarget)
        {
            EndCurrentAction();
            return;
        }

        animator.SetBool("IsMoving", true);

        // 水平方向へ移動（Y軸は固定なので verticalMoveSpeed の計算は省略可能だが残して固定高さに吸着させる形に）
        float verticalMoveDir = fixedHorizontalHeight - transform.position.y;
        float yVel = 0f;
        if (Mathf.Abs(verticalMoveDir) > 0.1f)
        {
            yVel = Mathf.Sign(verticalMoveDir) * verticalMoveSpeed;
        }

        rb.linearVelocity = new Vector2(horizontalDirection * horizontalMoveSpeed, yVel);
    }

    private void UpdateAttackPattern()
    {
        stateTimer -= Time.deltaTime;
        attackCooldownTimer -= Time.deltaTime;

        // 3回の攻撃をすべて終え、最後の攻撃後インターバルが経過するまで待ってから次の行動へ
        if (currentAttackCount >= 3 && attackCooldownTimer <= 0f)
        {
            EndCurrentAction();
            return;
        }

        // 時間切れによる終了は、まだ攻撃を一度も開始していない時のみ
        if (stateTimer <= 0f && currentAttackCount == 0)
        {
            EndCurrentAction();
            return;
        }

        // 3回攻撃を終えた後は、最後のクールダウンが終わるまでその場で待機する
        if (currentAttackCount >= 3)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsMoving", false);
            return;
        }

        if (player == null)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsMoving", false);
            return;
        }

        // 攻撃範囲内にプレイヤーがいるか判定
        bool inAttackRange = false;
        if (attackSensor != null)
        {
            Collider2D hit = Physics2D.OverlapCircle(attackSensor.position, attackRange, playerLayer);
            if (hit != null && hit.CompareTag("Player"))
            {
                inAttackRange = true;
            }
        }

        if (inAttackRange)
        {
            // 攻撃範囲にいるなら移動を停止
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsMoving", false);

            // クールダウンが明けていれば攻撃
            if (attackCooldownTimer <= 0f)
            {
                animator.SetTrigger("Attack");
                currentAttackCount++;
                attackCooldownTimer = Random.Range(minAttackCooldown, maxAttackCooldown);
            }
        }
        else
        {
            // 攻撃範囲にいない場合はプレイヤーを追跡
            MoveTowardsPlayer();
        }
    }

    private void EndCurrentAction()
    {
        currentState = BossState.ChooseAction;
        stateTimer = .1f; // 次の行動までの待機時間
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("IsMoving", false);
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        if (Vector2.Distance(player.position, transform.position) < 0.1f)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsMoving", false);
            return;
        }

        rb.linearVelocity = direction * chaseSpeed;
        animator.SetBool("IsMoving", true);

        // 進行方向に向きを切り替え (X方向の移動成分で判断)
        if (direction.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (direction.x < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void FindPlayerInRange()
    {
        if (player != null) return;

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

    private void OnDrawGizmosSelected()
    {
        if (attackSensor != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackSensor.position, attackRange);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // 水平移動の開始地点と高さの範囲を表示
        Gizmos.color = Color.cyan;
        if (startXPositions != null)
        {
            foreach (float x in startXPositions)
            {
                // Y軸の最小・最大位置
                Vector3 bottomPos = new Vector3(x, minHeight, 0);
                Vector3 topPos = new Vector3(x, maxHeight, 0);

                // 開始X座標上のY範囲を線で結ぶ
                Gizmos.DrawLine(bottomPos, topPos);

                // 目安となる球体を描画
                Gizmos.DrawSphere(bottomPos, 0.2f);
                Gizmos.DrawSphere(topPos, 0.2f);
            }

            // 固定高さ（水平移動時）のラインを表示
            if (startXPositions.Length > 0)
            {
                Gizmos.color = Color.green;
                float minX = Mathf.Min(startXPositions);
                float maxX = Mathf.Max(startXPositions);
                
                Vector3 leftPos = new Vector3(minX, fixedHorizontalHeight, 0);
                Vector3 rightPos = new Vector3(maxX, fixedHorizontalHeight, 0);
                
                // 開始・終了地点を結ぶ横線
                Gizmos.DrawLine(leftPos, rightPos);

                // 固定高さ上のポイントに球体を描画
                foreach (float x in startXPositions)
                {
                    Gizmos.DrawSphere(new Vector3(x, fixedHorizontalHeight, 0), 0.25f);
                }
            }
        }
    }


    [Header("Audio")]
    public AudioClip attackSE;
    // アニメーションイベントから呼び出す用
    public void PlayAttackSE()
    {
        if (SoundManager.Instance != null && attackSE != null)
        {
            SoundManager.Instance.PlaySE(attackSE);
        }
    }
}
