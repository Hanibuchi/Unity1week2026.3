using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueTrigger))]
public class UnderTheSeaEvent : MonoBehaviour
{
    [Header("カメラの対象にしたいオブジェクト")]
    [SerializeField] private Transform cameraTarget;

    [Header("画面遷移ゲート")]
    [SerializeField] private ScreenTransitionGate screenTransitionGate;

    [Header("ダイアログ開始までの遅延秒数")]
    [SerializeField] private float dialogueStartDelay = 1.0f;

    private DialogueTrigger dialogueTrigger;

    private void Awake()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();
        SetupDialogue();
    }

    /// <summary>
    /// イベントをスタートさせる（カメラの対象もここで設定）
    /// </summary>
    public void StartEvent()
    {
        // カメラの対象を設定
        if (cameraTarget != null && CameraController.Instance != null)
        {
            CameraController.Instance.SetTrackingTarget(cameraTarget);
        }

        // 海を泳ぐアニメーションをトリガー
        var animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Swim");
        }
        else
        {
            Debug.LogWarning("UnderTheSeaEvent: Animatorコンポーネントが見つかりません。");
        }

        // 画面遷移をトリガー
        if (screenTransitionGate != null && cameraTarget != null)
        {
            screenTransitionGate.StartTransition(cameraTarget);
        }
        else
        {
            Debug.LogWarning("UnderTheSeaEvent: screenTransitionGateまたはcameraTargetが設定されていません。");
        }

        // 一定時間後にダイアログ開始
        StartCoroutine(StartDialogueAfterDelay());
    }

    private System.Collections.IEnumerator StartDialogueAfterDelay()
    {
        yield return new WaitForSeconds(dialogueStartDelay);
        if (dialogueTrigger != null)
        {
            dialogueTrigger.Interact();
        }
    }

    private void SetupDialogue()
    {
        var settings = CommonGameSettings.Settings ?? Resources.Load<GameSettingsData>("GameSettings");
        if (settings == null)
        {
            Debug.LogWarning("UnderTheSeaEvent: GameSettingsが見つかりません。");
            return;
        }

        var nodes = new List<DialogueNode>
        {
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "うわぁ……！海の中って、こんなにキラキラしてるんだべか。おら、お魚さんと一緒に泳いでるみたいだぁ。",
                speakerSprite = settings.taroFaceSurprise
            },
            new DialogueNode
            {
                speakerName = "カメ",
                text = "太郎さん、あまり口を開けないでくださいね。海水が入りますよ。……まあ、あなたのその驚きよう、案内役としては悪い気はしませんが。",
                speakerSprite = settings.kameFaceNormal
            },
            new DialogueNode
            {
                speakerName = "カメ",
                text = "そろそろですよ、太郎さん。私たちの目的地、竜宮城が見えてきました。",
                speakerSprite = settings.kameFaceJoy
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "……！なんだべ、あれ……！あんなにデカくて光ってる建物、見たことねぇ！お城が……本当にお城が海の底にあるんだべか！",
                speakerSprite = settings.taroFaceSurprise
            }
        };

        Debug.Log("UnderTheSeaEvent: Setting up dialogue with " + nodes.Count + " nodes.");
        if (dialogueTrigger != null)
        {
            dialogueTrigger.SetDialogueNodes(nodes);
        }
        else
        {
            Debug.LogWarning("UnderTheSeaEvent: DialogueTriggerコンポーネントが見つかりません。");
        }
    }
}
