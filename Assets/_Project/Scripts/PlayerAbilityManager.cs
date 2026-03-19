using UnityEngine;
using System;

/// <summary>
/// プレイヤーの能力（アビリティ）の状況を管理するマネージャー。
/// </summary>
public class PlayerAbilityManager : MonoBehaviour
{
    public static PlayerAbilityManager Instance { get; private set; }

    [SerializeField]
    private PlayerAbilities currentAbilities = new PlayerAbilities();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// セーブデータなどからロードしたアビリティ状態を適用し、PlayerControllerに反映します。
    /// </summary>
    public void LoadAndApplyAbilities()
    {
        if (FlagManager.Instance != null)
        {
            currentAbilities.canJetDash = FlagManager.Instance.GetFlag("HasJetDash");
            currentAbilities.canAttackDown = FlagManager.Instance.GetFlag("HasAttackDown");
            currentAbilities.canIncreaseAttack = FlagManager.Instance.GetFlag("HasIncreaseAttack");
        }
        else
        {
            Debug.LogWarning("[PlayerAbilityManager] FlagManagerが見つかりません。能力のロードをスキップします。");
        }

        ApplyAbilitiesToPlayer();
        Debug.Log("[PlayerAbilityManager] 能力をロードし、プレイヤーに適用しました。");
    }

    /// <summary>
    /// 特定のアビリティを解放（追加）します。
    /// </summary>
    /// <param name="abilityName">解放するアビリティ名</param>
    public void UnlockAbility(string abilityName)
    {
        switch (abilityName)
        {
            case "JetDash":
                currentAbilities.canJetDash = true;
                break;
            case "AttackDown":
                currentAbilities.canAttackDown = true;
                break;
            case "IncreaseAttack":
                currentAbilities.canIncreaseAttack = true;
                break;
            default:
                Debug.LogWarning($"[PlayerAbilityManager] 不明なアビリティ名が指定されました: {abilityName}");
                return;
        }

        if (FlagManager.Instance != null)
        {
            FlagManager.Instance.SetFlag($"Has{abilityName}", true);
        }

        ApplyAbilitiesToPlayer();
        Debug.Log($"[PlayerAbilityManager] アビリティ '{abilityName}' を解放しました。");
    }

    /// <summary>
    /// 現在のアビリティ状態をプレイヤーに反映させます。
    /// </summary>
    private void ApplyAbilitiesToPlayer()
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.UpdateAbilities(currentAbilities);
        }
        else
        {
            Debug.LogWarning("[PlayerAbilityManager] PlayerControllerが存在しないため、アビリティを適用できませんでした。");
        }
    }
}
