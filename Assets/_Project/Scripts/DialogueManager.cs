using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueNode
{
    public Sprite speakerSprite;
    public string speakerName;
    [TextArea(3, 5)]
    public string text;

    public bool hasChoices;
    public string choice1Text;
    public string choice2Text;

    // 選択肢ごとの次のセリフ群（空なら終了または呼び出し元に制御を返すなど）
    public List<DialogueNode> choice1NextNodes;
    public List<DialogueNode> choice2NextNodes;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    private bool isPlaying = false;
    private Action onSequenceComplete;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// セリフのシーケンスの再生を開始します。
    /// </summary>
    public void StartDialogue(List<DialogueNode> nodes, Action onComplete = null)
    {
        if (isPlaying || nodes == null || nodes.Count == 0) return;

        isPlaying = true;
        onSequenceComplete = onComplete;

        // プレイヤーの操作を無効化する
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.SetControlEnabled(false);
        }

        StartCoroutine(PlayDialogueSequence(nodes));
    }

    private IEnumerator PlayDialogueSequence(List<DialogueNode> nodes)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            var node = nodes[i];
            bool isWaitingForInput = true;

            // DialogueUI にセリフを表示させる
            var dialogueUI = UIManager.Instance.GetView<DialogueUI>();
            if (dialogueUI != null)
            {
                dialogueUI.PlayDialogue(node.speakerSprite, node.speakerName, node.text, () =>
                {
                    isWaitingForInput = false;
                });
            }
            else
            {
                isWaitingForInput = false;
            }

            // セリフの表示完了と入力を待機
            while (isWaitingForInput)
            {
                yield return null;
            }

            // 選択肢がある場合
            if (node.hasChoices)
            {
                int selectedChoice = 0;
                bool isChoiceSelected = false;

                if (dialogueUI != null)
                {
                    dialogueUI.ShowChoices(node.choice1Text, node.choice2Text, (choiceIndex) => 
                    {
                        selectedChoice = choiceIndex;
                        isChoiceSelected = true;
                    });
                }
                else
                {
                    Debug.LogWarning("DialogueUIが見つからないため、強制的に選択肢1へ進みます");
                    selectedChoice = 1;
                    isChoiceSelected = true;
                }

                while (!isChoiceSelected)
                {
                    yield return null;
                }

                // 選ばれた選択肢に応じた次のノード群を再生（再帰または現在のリストを置き換え）
                List<DialogueNode> nextNodes = selectedChoice == 1 ? node.choice1NextNodes : node.choice2NextNodes;
                if (nextNodes != null && nextNodes.Count > 0)
                {
                    yield return StartCoroutine(PlayDialogueSequence(nextNodes));
                }

                // 選択肢の後は現在のシーケンスを終了するか、後続の設計によります
                break;
            }
        }

        EndDialogue();
    }

    private void EndDialogue()
    {
        isPlaying = false;

        // ダイアログUIを非表示にする
        UIManager.Instance.Hide<DialogueUI>();

        // プレイヤーの操作を有効化する
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.SetControlEnabled(true);
        }

        onSequenceComplete?.Invoke();
        onSequenceComplete = null;
    }

    public List<DialogueNode> testNodes;
    public void Test()
    {
        StartDialogue(testNodes, () => Debug.Log("セリフのシーケンスが完了しました！"));
    }
}
