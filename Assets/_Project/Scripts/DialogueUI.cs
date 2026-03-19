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
    private bool isWaitingForPageSkip = false;
    private bool skipRequested = false;
    private int currentVisibleIndex = 0;
    private int totalCharacters = 0;
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
            // タイピング中なら現在のページ部分を一気に表示する
            if (isTyping)
            {
                skipRequested = true;
            }
            // ページがいっぱいで待機中なら、次のページへ進む
            else if (isWaitingForPageSkip)
            {
                isWaitingForPageSkip = false;
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
        isWaitingForPageSkip = false;
        skipRequested = false;

        // TextMeshProのページネーション機能を有効化する
        dialogueText.overflowMode = TextOverflowModes.Page;
        dialogueText.text = dialogue;
        dialogueText.ForceMeshUpdate(); // テキストの情報を計算するためにメッシュを更新

        totalCharacters = dialogueText.textInfo.characterCount;
        dialogueText.pageToDisplay = 1;
        dialogueText.maxVisibleCharacters = 0;
        currentVisibleIndex = 0;

        while (currentVisibleIndex < totalCharacters)
        {
            // 現在の文字がどのページに属しているか取得 (pageNumberは0から始まるので＋1する)
            int charPage = dialogueText.textInfo.characterInfo[currentVisibleIndex].pageNumber + 1;

            // ページが変わる場合、入力待ち状態へ
            if (charPage > dialogueText.pageToDisplay)
            {
                isTyping = false;
                isWaitingForPageSkip = true;

                // 次へ進む入力があるまで待機
                while (isWaitingForPageSkip)
                {
                    yield return null;
                }

                // 次のページへ
                isTyping = true;
                skipRequested = false;
                dialogueText.pageToDisplay = charPage;
            }

            dialogueText.maxVisibleCharacters = currentVisibleIndex + 1;
            currentVisibleIndex++;

            // スキップ中でなければ音を鳴らして待機
            if (!skipRequested)
            {
                if (typingSound != null && SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlaySE(typingSound);
                }

                yield return new WaitForSeconds(typingSpeed);
            }
        }

        dialogueText.maxVisibleCharacters = totalCharacters; // 最後まで表示
        isTyping = false;
        isWaitingForPageSkip = false;
        skipRequested = false;
        typingCoroutine = null;
    }

    public Sprite testSprite; // テスト用のキャラクタースプライト
    public string testName = "テストキャラ";
    public string testDialogue = "これはテストのセリフです。あぇsjふぉいうぇっかsdlkfjははｓｋｄｆｈｐくぃえうｒｌｋｑｗｊへｒｋｊはｓｄｆはｄｊｋｓｋｊｋｚｋｘｊｃｖｌｋじゃｈｓｐぢふｈくぇえｋｆjはsldkfjhlsdkjhfkljhsdfぃうあyうぇいるhうぇr";
    public void Test()
    {
        PlayDialogue(testSprite, testName, testDialogue, () => Debug.Log("セリフが完了しました！"));
    }
}
