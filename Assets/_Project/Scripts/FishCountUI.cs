using TMPro;
using UnityEngine;

/// <summary>
/// 魚の切り身の数を表示するUI
/// </summary>
public class FishCountUI : UIView
{
    [SerializeField] private TextMeshProUGUI countText;

    private void Update()
    {
        if (FishCountManager.Instance != null && countText != null)
        {
            countText.text = FishCountManager.Instance.FishCount.ToString();
        }
    }

    /// <summary>
    /// 魚の切り身の数をUIに反映します。
    /// （手動更新用。通常はUpdateで自動更新されます）
    /// </summary>
    /// <param name="count">表示する数</param>
    public void SetCount(int count)
    {
        if (countText != null)
        {
            countText.text = count.ToString();
        }
    }
}
