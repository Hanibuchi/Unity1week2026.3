using UnityEngine;

public class BugController : EnemyController
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public LayerMask groundLayer;
    public Transform wallCheck;
    public float wallCheckDistance = 0.5f;

    private int moveDirection = 1; // 初期設定では右向きを想定
    [SerializeField] bool flip = false;

    void Start()
    {
        // scale.xの符号で進行方向を決定
        if (flip)
        {
            Flip();
            moveDirection *= -1;//
        }
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        CheckWall();
        Move();
    }

    private void CheckWall()
    {
        if (wallCheck == null) return;

        // 前方に壁 (Ground) があるか判定
        Vector2 checkDirection = isFacingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, checkDirection, wallCheckDistance, groundLayer);

        if (hit.collider != null)
        {
            // 壁にぶつかったら反転して進行方向を逆にする
            Flip();
            moveDirection *= -1;
        }
    }

    protected override void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x = isFacingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    private void Move()
    {
        // Y軸の力はそのままに、X軸方向のみ移動させる
        rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);

        if (animator != null)
        {
            animator.SetBool("IsMoving", true);
        }
    }

    // エディタ上で壁判定のRayを視覚化する
    private void OnDrawGizmosSelected()
    {
        if (wallCheck != null)
        {
            Gizmos.color = Color.blue;
            Vector2 checkDirection = isFacingRight ? Vector2.right : Vector2.left;
            Gizmos.DrawRay(wallCheck.position, checkDirection * wallCheckDistance);
        }
    }
}
