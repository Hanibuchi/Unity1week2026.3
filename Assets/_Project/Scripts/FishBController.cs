using UnityEngine;

public class FishBController : FishAController
{
    [Header("Attack Settings")]
    public Transform attackSensor;
    public float attackRange = 1f;
    public LayerMask playerLayer;
    public float attackWaitTime = 2f; // 攻撃後、その場に留まる時間

    [Header("Audio")]
    public AudioClip attackSE;

    private float waitTimer = 0f;

    protected override void Update()
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

        // 攻撃範囲のチェック
        if (CheckAttack())
        {
            // 攻撃を行った（もしくは待機状態に入った）場合は移動をスキップ
            return;
        }

        // 特に攻撃状態でなければFishAの通常の移動(ランダム移動)を行う
        base.Update();
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
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsMoving", false);
            
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

    // 攻撃センサーの範囲をエディタ上で視覚化する
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 drawPos = attackSensor != null ? attackSensor.position : transform.position;
        Gizmos.DrawWireSphere(drawPos, attackRange);
    }
}
