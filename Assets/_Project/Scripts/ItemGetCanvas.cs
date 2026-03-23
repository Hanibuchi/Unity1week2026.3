using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using UnityEngine.InputSystem;

public class ItemGetCanvas : UIView
{
    [Header("UI References")]
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemDescriptionText;

    [Header("Settings")]
    [SerializeField] private AudioClip getItemSound;

    [Header("Input")]
    [Tooltip("閉じるためのアクション名 (Project-wide Actionsに登録されている名前)")]
    [SerializeField] private string closeActionName = "UI/Submit";

    private InputAction closeAction;
    private Action onShowComplete;
    private bool isShowing = false;

    private void Start()
    {
        if (InputSystem.actions != null)
        {
            closeAction = InputSystem.actions.FindAction(closeActionName);
            if (closeAction != null)
            {
                closeAction.Enable();
            }
            else
            {
                Debug.LogWarning($"[ItemGetCanvas] Action '{closeActionName}' が見つかりませんでした。Project SettingsのInput Systemを確認してください。");
            }
        }
        else
        {
            Debug.LogWarning("[ItemGetCanvas] InputSystem.actions が null です。Project Settings -> Input System Package -> Default Actions が設定されているか確認してください。");
        }
    }

    private void Update()
    {
        if (isShowing && closeAction != null && closeAction.WasPressedThisFrame())
        {
            Hide();
            isShowing = false;
            onShowComplete?.Invoke();
            onShowComplete = null;
        }
    }

    public void ShowItem(Sprite sprite, string itemName, string itemDescription, Action onComplete = null)
    {
        if (!_isVisible)
        {
            Show();
        }

        if (itemImage != null)
        {
            itemImage.sprite = sprite;
            itemImage.gameObject.SetActive(sprite != null);
        }

        if (itemNameText != null)
        {
            itemNameText.text = itemName;
        }

        if (itemDescriptionText != null)
        {
            itemDescriptionText.text = itemDescription;
        }

        onShowComplete = onComplete;
        isShowing = true;

        if (getItemSound != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySE(getItemSound);
        }
    }

    // テスト用
    public Sprite testSprite;
    public string testName = "テストアイテム";
    public string testDescription = "これはテスト用のアイテム説明です。";
    public void Test()
    {
        ShowItem(testSprite, testName, testDescription, () => Debug.Log("アイテム表示が完了しました！"));
    }
}
