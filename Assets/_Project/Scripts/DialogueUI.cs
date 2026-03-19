using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueUI : UIView
{
    [Header("UI References")]
    [SerializeField] private Image characterImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private AudioClip typingSound;

    [Header("Input")]
    [Tooltip("セリフ送りをスキップするためのアクション名 (Project-wide Actionsに登録されている名前)")]
    [SerializeField] private string skipActionName = "UI/Submit";

    private InputAction skipAction;
    private Coroutine typingCoroutine;
    private string currentDialogue = "";
    private bool isTyping = false;
    private Action onDialogueComplete;

    private void Start()
    {
        if (InputSystem.actions != null)
        {
            skipAction = InputSystem.actions.FindAction(skipActionName);
            if (skipAction != null)
            {
                skipAction.Enable(); // アクションが有効化されていない可能性があるため明示的に有効化
            }
            else
            {
                Debug.LogWarning($"[DialogueUI] Action '{skipActionName}' が見つかりませんでした。Project SettingsのInput Systemを確認してください。");
            }
        }
        else
        {
            Debug.LogWarning("[DialogueUI] InputSystem.actions が null です。Project Settings -> Input System Package -> Default Actions が設定されているか確認してください。");
        }
    }

    private void Update()
    {
        if (skipAction != null && skipAction.WasPressedThisFrame())
        {
            // タイピング中なら一気に表示する
            if (isTyping)
            {
                SkipTyping();
            }
            // タイピング完了後なら次のアクション（コールバック）を実行する
            else if (onDialogueComplete != null)
            {
                var callback = onDialogueComplete;
                onDialogueComplete = null; // 重複実行を防ぐためクリア
                callback.Invoke();
            }
        }
    }

    public void PlayDialogue(Sprite sprite, string characterName, string dialogue, Action onComplete = null)
    {
        if (!_isVisible)
        {
            Show();
        }

        if (characterImage != null)
        {
            characterImage.sprite = sprite;
            characterImage.gameObject.SetActive(sprite != null);
        }

        if (nameText != null)
        {
            nameText.text = characterName;
        }

        currentDialogue = dialogue;
        onDialogueComplete = onComplete;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeDialogueCoroutine(currentDialogue));
    }

    private IEnumerator TypeDialogueCoroutine(string dialogue)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in dialogue)
        {
            dialogueText.text += c;

            // 音を鳴らす
            if (typingSound != null && SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySE(typingSound);
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        typingCoroutine = null;
    }

    private void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        dialogueText.text = currentDialogue;
        isTyping = false;
    }

    public Sprite testSprite; // テスト用のキャラクタースプライト
    public string testName = "テストキャラ";
    public string testDialogue = "これはテストのセリフです。あぇsjふぉいうぇっかsdlkfjははｓｋｄｆｈｐくぃえうｒｌｋｑｗｊへｒｋｊはｓｄｆはｄｊｋｓｋｊｋｚｋｘｊｃｖｌｋじゃｈｓｐぢふｈくぇえｋｆjはsldkfjhlsdkjhfkljhsdfぃうあyうぇいるhうぇr";
    public void Test()
    {
        PlayDialogue(testSprite, testName, testDialogue, () => Debug.Log("セリフが完了しました！"));
    }
}
