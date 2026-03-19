using System;
using UnityEngine;
using UnityEngine.UI;

public class CreditUI : UIView
{
    [SerializeField] private Button closeButton;

    private Action _onClose;

    private void Awake()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }
    }

    public void Initialize(Action onClose)
    {
        _onClose = onClose;
    }

    private void OnCloseButtonClicked()
    {
        Hide();
        _onClose?.Invoke();
    }

    public override void Show()
    {
        base.Show();
        if (closeButton != null)
        {
            closeButton.Select();
        }
    }
}
