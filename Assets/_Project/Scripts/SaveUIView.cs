using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SaveUIView : UIView
{
    [Header("UI Elements")]
    [SerializeField] private Button closeButton;

    [Header("Save Slots")]
    [Tooltip("インスペクタから各セーブスロットを割り当ててください")]
    [SerializeField] private List<SaveSlot> saveSlots = new List<SaveSlot>();

    private void Awake()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }
    }

    public override void Show()
    {
        base.Show();
        RefreshSlots();

        // 最初のスロット（セーブ1）を選択状態にする
        if (saveSlots != null && saveSlots.Count > 0 && saveSlots[0] != null)
        {
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(saveSlots[0].gameObject);
            }
        }
    }

    /// <summary>
    /// 各セーブスロットの表示を更新します。
    /// 実際のセーブロードシステムに合わせてデータを取得するように変更してください。
    /// </summary>
    public void RefreshSlots()
    {
        for (int i = 0; i < saveSlots.Count; i++)
        {
            int saveNumber = i + 1;
            
            // FlagManagerの静的メソッドを使ってデータ有無をチェックします
            bool hasData = FlagManager.HasSaveData(saveNumber);

            if (hasData)
            {
                string playTime = SaveManager.GetFormattedPlayTime(saveNumber);
                string locationName = SaveManager.GetSavedLocation(saveNumber);
                
                saveSlots[i].Setup(saveNumber, playTime, locationName);
            }
            else
            {
                saveSlots[i].SetEmpty(saveNumber);
            }
        }
    }
}
