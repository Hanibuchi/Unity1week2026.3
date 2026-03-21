using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueTrigger))]
public class GroundEvent : MonoBehaviour
{
    [Header("画面遷移ゲート")]
    [SerializeField] private ScreenTransitionGate screenTransitionGate;

    [Header("SE再生までの遅延秒数")]
    [SerializeField] private float sePlayDelay = 0.5f;

    [Header("SE再生後、ダイアログ開始までの遅延秒数")]
    [SerializeField] private float dialogueStartDelayAfterSE = 1.0f;

    [Header("再生するSEクリップ")]
    [SerializeField] private AudioClip seClip;

    private DialogueTrigger dialogueTrigger;

    private void Awake()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();
        SetupDialogue();
    }

    /// <summary>
    /// 地上イベントをスタートさせる（画面遷移とダイアログ開始）
    /// </summary>
    public void StartEvent()
    {
        // 画面遷移をトリガー
        if (screenTransitionGate != null)
        {
            if (PlayerController.Instance != null)
            {
                screenTransitionGate.StartTransition(PlayerController.Instance.transform);
            }
            else
            {
                Debug.LogWarning("GroundEvent: PlayerController.Instanceが見つかりません。");
            }
        }
        else
        {
            Debug.LogWarning("GroundEvent: screenTransitionGateが設定されていません。");
        }

        // 一定時間後にダイアログ開始
        StartCoroutine(StartDialogueAfterDelay());
    }

    private System.Collections.IEnumerator StartDialogueAfterDelay()
    {
        // SE再生まで待機
        yield return new WaitForSeconds(sePlayDelay);

        // SE再生
        if (seClip != null)
        {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySE(seClip);
            }
            else
            {
                Debug.LogWarning("GroundEvent: SoundManager.Instanceが見つかりません。SE再生できません。");
            }
        }
        else
        {
            Debug.LogWarning("GroundEvent: seClipが設定されていません。");
        }

        // SE再生後さらに待機
        yield return new WaitForSeconds(dialogueStartDelayAfterSE);

        // ダイアログ開始
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
            Debug.LogWarning("GroundEvent: GameSettingsが見つかりません。");
            return;
        }

        var nodes = new List<DialogueNode>
        {
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "……。なんだべ、これ……！？",
                speakerSprite = settings.taroFaceConfusion
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "おらの大好きだった、キラキラ光る青い海が……真っ黒だべ。油が浮いて、変な臭いがして……これじゃお魚さんたち、みんな死んじまうだ！",
                speakerSprite = settings.taroFaceSadness
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "空の色までおかしいだ。お天道様はどこだ？紫色の雲が重たくのしかかって、まるでお天道様が死んじまったみたいだっぺ……。",
                speakerSprite = settings.taroFaceConfusion
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "ここ、本当におらのいた村だか……？",
                speakerSprite = settings.taroFaceConfusion
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "どこもかしこもみんなドブネズミみてぇな色してボロボロだっぺ……！何が起きたんだっぺ？",
                speakerSprite = settings.taroFaceSadness
            }
        };

        Debug.Log("GroundEvent: Setting up dialogue with " + nodes.Count + " nodes.");
        if (dialogueTrigger != null)
        {
            dialogueTrigger.SetDialogueNodes(nodes);
        }
        else
        {
            Debug.LogWarning("GroundEvent: DialogueTriggerコンポーネントが見つかりません。");
        }
    }
}
