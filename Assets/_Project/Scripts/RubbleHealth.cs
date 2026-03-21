using UnityEngine;

public class RubbleHealth : EnemyHealth
{
    public AudioClip rejectSE; // ダメージをはじいた時のSE

    public override void TakeDamage(int damageAmount, Vector3 hitPosition)
    {
        if (CanTakeDamage())
        {
            base.TakeDamage(damageAmount, hitPosition);
        }
        else
        {
            // ダメージをはじいた時のSEを鳴らす
            if (SoundManager.Instance != null && rejectSE != null)
            {
                SoundManager.Instance.PlaySE(rejectSE);
            }
        }
    }

    private bool CanTakeDamage()
    {
        // HasIncreaseAttackフラグが立っているときのみダメージを受ける
        return FlagManager.Instance != null && FlagManager.Instance.GetFlag(FlagManager.FlagKey.HasIncreaseAttack.ToString());
    }
}
