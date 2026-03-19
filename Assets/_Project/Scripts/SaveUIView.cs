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
            
            // TODO: 実際のセーブデータ管理クラスからデータを取得する処理に置き換えてください
            // 以下は表示テスト用のダミー処理です
            bool hasData = (i == 0); // 例として1つ目のスロットのみデータありとする

            if (hasData)
            {
                // ダミーデータ（例: セーブ1、12時間30分12秒、竜宮城入口）
                saveSlots[i].Setup(saveNumber, "12時間30分12秒", "竜宮城入口");
            }
            else
            {
                saveSlots[i].SetEmpty(saveNumber);
            }
        }
    }
}
