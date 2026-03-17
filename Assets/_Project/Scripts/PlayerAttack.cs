using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public int damage = 10;

    [Header("Events on Hit")]
    [Tooltip("敵に攻撃が当たった際にインスペクタから呼び出されるメソッドを登録します。")]
    public UnityEvent onHitEnemy;

    // 敵の体力クラスから攻撃が命中した際に通知されるメソッド
    public void OnHitTarget(EnemyHealth enemy)
    {
        // インスペクタから設定されたメソッドを実行する
        onHitEnemy?.Invoke();
    }
}
