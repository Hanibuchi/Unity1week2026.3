using System;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : UIView
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button creditButton;

    /// <summary>
    /// 初期化処理。スタートボタン押下時のアクションを受け取ります。
    /// </summary>
    /// <param name="onStartClicked">スタートボタンが押された時の処理</param>
    public void Initialize(Action onStartClicked)
    {
        if (startButton != null)
        {
            startButton.onClick.RemoveAllListeners();
            if (onStartClicked != null)
            {
                startButton.onClick.AddListener(() => onStartClicked.Invoke());
            }
        }

        if (creditButton != null)
        {
            creditButton.onClick.RemoveAllListeners();
            creditButton.onClick.AddListener(OnCreditButtonClicked);
        }
    }

    private void OnCreditButtonClicked()
    {
        // クレジットUIを表示
        if (UIManager.Instance != null)
        {
            var creditUI = UIManager.Instance.GetView<CreditUI>();
            if (creditUI != null)
            {
                SetButtonsInteractable(false);
                creditUI.Initialize(() => 
                {
                    SetButtonsInteractable(true);
                    if (startButton != null)
                    {
                        startButton.Select();
                    }
                });
                UIManager.Instance.Show<CreditUI>();
            }
        }
    }

    private void SetButtonsInteractable(bool interactable)
    {
        if (startButton != null) startButton.interactable = interactable;
        if (creditButton != null) creditButton.interactable = interactable;
    }

    public override void Show()
    {
        base.Show();
        if (startButton != null)
        {
            startButton.Select();
        }
    }
}
