using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : UIView
{
    [Header("UI References")]
    [Tooltip("ハートを並べる親オブジェクト")]
    [SerializeField] private Transform heartContainer;
    
    [Tooltip("満たされたハートのプレハブ")]
    [SerializeField] private GameObject fullHeartPrefab;
    
    [Tooltip("空のハートのプレハブ")]
    [SerializeField] private GameObject emptyHeartPrefab;

    [Header("Item Collection UI")]
    [Tooltip("回復アイテムの収集状況を示すImage (Image TypeをFilledに設定)")]
    [SerializeField] private Image healItemProgressImage;

    private void Start()
    {
        if (PlayerHealth.Instance != null)
        {
            // 初期体力の同期
            UpdateHealthUI(PlayerHealth.Instance.CurrentHealth, PlayerHealth.Instance.MaxHealth);
            // イベントの購読
            PlayerHealth.Instance.OnHealthChanged.AddListener(UpdateHealthUI);
            PlayerHealth.Instance.OnHealItemProgressChanged.AddListener(UpdateHealItemProgressUI);
        }
        else
        {
            Debug.LogWarning("[PlayerHealthUI] PlayerHealthのインスタンスが見つかりません。");
        }
    }

    private void OnDestroy()
    {
        // オブジェクト破棄時にイベントの購読を解除
        if (PlayerHealth.Instance != null)
        {
            PlayerHealth.Instance.OnHealthChanged.RemoveListener(UpdateHealthUI);
            PlayerHealth.Instance.OnHealItemProgressChanged.RemoveListener(UpdateHealItemProgressUI);
        }
    }

    /// <summary>
    /// 体力UIを更新する
    /// </summary>
    public void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        if (heartContainer == null || fullHeartPrefab == null || emptyHeartPrefab == null)
        {
            Debug.LogWarning("[PlayerHealthUI] インスペクタの参照が設定されていません。");
            return;
        }

        // 既存のハートをすべて削除する
        foreach (Transform child in heartContainer)
        {
            Destroy(child.gameObject);
        }

        // 新しい体力に応じてハートを再生成する
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject prefabToInstantiate = (i < currentHealth) ? fullHeartPrefab : emptyHeartPrefab;
            Instantiate(prefabToInstantiate, heartContainer);
        }
    }

    /// <summary>
    /// アイテムの収集度合いに合わせてImageのFill amountを更新する
    /// </summary>
    public void UpdateHealItemProgressUI(int currentCount, int maxRequiredCount)
    {
        if (healItemProgressImage != null && maxRequiredCount > 0)
        {
            healItemProgressImage.fillAmount = (float)currentCount / maxRequiredCount;
        }
    }
}
