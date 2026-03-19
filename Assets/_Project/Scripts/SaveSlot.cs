using UnityEngine;
using TMPro;

public class SaveSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text saveNumberText;
    [SerializeField] private TMP_Text playTimeText;
    [SerializeField] private TMP_Text locationText;

    /// <summary>
    /// セーブデータが存在する場合の表示設定
    /// </summary>
    public void Setup(int saveNumber, string playTime, string location)
    {
        if (saveNumberText != null) saveNumberText.text = $"セーブ {saveNumber}";
        if (playTimeText != null) playTimeText.text = playTime;
        if (locationText != null) locationText.text = location;
    }

    /// <summary>
    /// セーブデータが存在しない（空）場合の表示設定
    /// </summary>
    public void SetEmpty(int saveNumber)
    {
        if (saveNumberText != null) saveNumberText.text = $"セーブ {saveNumber}";
        if (playTimeText != null) playTimeText.text = "--時間--分--秒";
        if (locationText != null) locationText.text = "NO DATA";
    }
}
