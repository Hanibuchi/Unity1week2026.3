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

    private void Start()
    {
        if (PlayerHealth.Instance != null)
        {
            // 初期体力の同期
            UpdateHealthUI(PlayerHealth.Instance.CurrentHealth, PlayerHealth.Instance.MaxHealth);
            // イベントの購読
            PlayerHealth.Instance.OnHealthChanged.AddListener(UpdateHealthUI);
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
}
