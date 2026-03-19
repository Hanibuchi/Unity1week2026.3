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

    // TODO: 2択のUIへの参照も必要になります（現在は未実装と仮定）
    // [SerializeField] private ChoiceUI choiceUI; 

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
    /// 再生中は TimeScale を 0 にして時間を止めます。
    /// </summary>
    public void StartDialogue(List<DialogueNode> nodes, Action onComplete = null)
    {
        if (isPlaying || nodes == null || nodes.Count == 0) return;

        isPlaying = true;
        onSequenceComplete = onComplete;

        // 時間を止める
        TimeScaleManager.Instance.SetTimeScale(0f, (int)TimeScaleManager.TimeScalePriority.Menu, this);

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

                // TODO: ここで実際のChoiceUIを表示して選択を待機する処理を実装します。
                // 例: choiceUI.ShowChoices(node.choice1Text, node.choice2Text, (choiceIndex) => { selectedChoice = choiceIndex; isChoiceSelected = true; });

                // 仮の実装（1秒後に自動で選択肢1を選ぶ）
                Debug.Log($"選択肢待機中: 1.{node.choice1Text} / 2.{node.choice2Text}");
                yield return new WaitForSecondsRealtime(1f);
                selectedChoice = 1;
                isChoiceSelected = true;

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

        // 時間の停止を解除する
        TimeScaleManager.Instance.RemoveRequest(this);

        onSequenceComplete?.Invoke();
        onSequenceComplete = null;
    }

    public List<DialogueNode> testNodes;
    public void Test()
    {
        StartDialogue(testNodes, () => Debug.Log("セリフのシーケンスが完了しました！"));
    }
}
