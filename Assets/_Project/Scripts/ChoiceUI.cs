using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ChoiceUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button choice1Button;
    [SerializeField] private TMP_Text choice1Text;
    
    [SerializeField] private Button choice2Button;
    [SerializeField] private TMP_Text choice2Text;

    private void Awake()
    {
        // 初期状態では非表示にしておく
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 選択肢を表示して入力を待機します。
    /// </summary>
    /// <param name="choice1">選択肢1のテキスト</param>
    /// <param name="choice2">選択肢2のテキスト</param>
    /// <param name="onChoiceSelected">選択された際のコールバック(1 or 2 を返す)</param>
    public void ShowChoices(string choice1, string choice2, Action<int> onChoiceSelected)
    {
        gameObject.SetActive(true);

        if (choice1Text != null) choice1Text.text = choice1;
        if (choice2Text != null) choice2Text.text = choice2;

        // choice1のボタンを最初に選択された状態にする
        if (choice1Button != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(choice1Button.gameObject);
        }

        // イベントリスナーを一度クリアして付け直す
        if (choice1Button != null)
        {
            choice1Button.onClick.RemoveAllListeners();
            choice1Button.onClick.AddListener(() => 
            {
                gameObject.SetActive(false);
                onChoiceSelected?.Invoke(1);
            });
        }

        if (choice2Button != null)
        {
            choice2Button.onClick.RemoveAllListeners();
            choice2Button.onClick.AddListener(() => 
            {
                gameObject.SetActive(false);
                onChoiceSelected?.Invoke(2);
            });
        }
    }
}
