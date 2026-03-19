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

    private void Start()
    {
        if (InputSystem.actions != null)
        {
            skipAction = InputSystem.actions.FindAction(skipActionName);
        }
    }

    private void Update()
    {
        // タイピング中かつスキップ入力があった場合、全テキストを一気に表示
        if (isTyping && skipAction != null && skipAction.WasPressedThisFrame())
        {
            SkipTyping();
        }
    }

    public void PlayDialogue(Sprite sprite, string characterName, string dialogue)
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

    public void Test()
    {
        PlayDialogue(null, "テストキャラ", "これはテストのセリフです。");
    }
}
