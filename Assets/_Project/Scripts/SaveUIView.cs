using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SaveUIView : UIView
{
    [Header("UI Elements")]
    [SerializeField] private Button closeButton;
    [SerializeField] private CanvasGroup mainCanvasGroup;

    [Header("Save Slots")]
    [Tooltip("インスペクタから各セーブスロットを割り当ててください")]
    [SerializeField] private List<SaveSlot> saveSlots = new List<SaveSlot>();

    private void Awake()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(() =>
            {
                if (SaveManager.Instance != null)
                {
                    SaveManager.Instance.CloseMenu();
                }
                else
                {
                    Hide();
                }
            });
        }
    }

    public override void Show()
    {
        base.Show();
        SetInteractable(true);
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

    /// <summary>
    /// UI全体の操作可否を切り替えます。
    /// 例えばダイアログ表示中などに操作を無効化する際に使用します。
    /// </summary>
    public void SetInteractable(bool isInteractable)
    {
        if (mainCanvasGroup != null)
        {
            mainCanvasGroup.interactable = isInteractable;
            mainCanvasGroup.blocksRaycasts = isInteractable;
        }
    }

    /// <summary>
    /// 指定されたスロット番号を選択状態にします
    /// </summary>
    public void SelectSlot(int slotNumber)
    {
        int index = slotNumber - 1;
        if (saveSlots != null && index >= 0 && index < saveSlots.Count && saveSlots[index] != null)
        {
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(saveSlots[index].gameObject);
            }
        }
    }
}
