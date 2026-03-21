using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueTrigger))]
public class BanquetEvent : MonoBehaviour
{
    [Header("画面遷移ゲート")]
    [SerializeField] private ScreenTransitionGate screenTransitionGate;

    [Header("ダイアログ開始までの遅延秒数")]
    [SerializeField] private float dialogueStartDelay = 1.0f;

    [Header("宴室への画面遷移ゲート")]
    [SerializeField] private ScreenTransitionGate banquetRoomTransitionGate;

    private DialogueTrigger dialogueTrigger;

    private void Awake()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();
        SetupDialogue();
    }

    /// <summary>
    /// 宴イベントをスタートさせる（画面遷移とダイアログ開始）
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
                Debug.LogWarning("BanquetEvent: PlayerController.Instanceが見つかりません。");
            }
        }
        else
        {
            Debug.LogWarning("BanquetEvent: screenTransitionGateが設定されていません。");
        }

        // 一定時間後にダイアログ開始
        StartCoroutine(StartDialogueAfterDelay());

        // ダイアログ終了時に宴終了フラグを立てる
        if (dialogueTrigger != null && FlagManager.Instance != null)
        {
            dialogueTrigger.onDialogueEnd += SetBanquetEndedFlag;
        }

    }

    private void SetBanquetEndedFlag()
    {
        if (FlagManager.Instance != null)
        {
            FlagManager.Instance.SetFlag(FlagManager.FlagKey.HasBanquetEnded.ToString(), true);
            Debug.Log("BanquetEvent: HasBanquetEndedフラグをtrueに設定しました。");
        }
        // 一度だけ実行されるように解除
        if (dialogueTrigger != null)
        {
            dialogueTrigger.onDialogueEnd -= SetBanquetEndedFlag;
        }

        // 宴室への遷移を実行
        if (banquetRoomTransitionGate != null && PlayerController.Instance != null)
        {
            banquetRoomTransitionGate.StartTransition(PlayerController.Instance.transform);
        }
        else
        {
            Debug.LogWarning("BanquetEvent: 宴室へのScreenTransitionGateまたはPlayerController.Instanceが設定されていません。");
        }
    }

    private System.Collections.IEnumerator StartDialogueAfterDelay()
    {
        // yield return new WaitForSeconds(dialogueStartDelay);
        if (dialogueTrigger != null)
        {
            dialogueTrigger.Interact();
        }
        yield break;
    }

    private void SetupDialogue()
    {
        var settings = CommonGameSettings.Settings ?? Resources.Load<GameSettingsData>("GameSettings");
        if (settings == null)
        {
            Debug.LogWarning("BanquetEvent: GameSettingsが見つかりません。");
            return;
        }

        var nodes = new List<DialogueNode>
        {
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "たまげたなぁ。村の庄屋様のお屋敷よりもずっとずっと立派だっぺ。まるで夢を見てるみたいだ。",
                speakerSprite = settings.taroFaceSurprise
            },
            new DialogueNode
            {
                speakerName = "おとひめ",
                text = "おかえりなさい、カメ。ずいぶん戻るのが遅いから、地上で人間に捕まってスープにでもされたのかと心配したわ。",
                speakerSprite = settings.otohimeFaceNormal
            },
            new DialogueNode
            {
                speakerName = "カメ",
                text = "乙姫様、申し訳ありません。実は、地上の子供たちに捕まり、危ういところをこの浦島太郎さんに救われました。",
                speakerSprite = settings.kameFaceNormal
            },
            new DialogueNode
            {
                speakerName = "おとひめ",
                text = "まあ。あなたがカメを助けてくれたのね。ありがとう、太郎さん。外の世界は野蛮な者ばかりだと思っていたけれど、あなたのような優しい方もいるのね。カメを助けてくれて、本当にありがとう。",
                speakerSprite = settings.otohimeFaceJoy
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "おら、難しいことはわがんね。でも、困ってるやつがいたら助けるのは、当たり前のことだっぺ。",
                speakerSprite = settings.taroFaceNormal
            },
            new DialogueNode
            {
                speakerName = "おとひめ",
                text = "損得もなしに動けるなんて、あなたには不思議な強さがあるのね。とても気に入ったわ。",
                speakerSprite = settings.otohimeFaceNormal
            },
            new DialogueNode
            {
                speakerName = "おとひめ",
                text = "さあ、遠いところをよく来てくれました。今日はあなたの勇気を称えて、盛大な宴を開きましょう！おいしい料理と踊りを楽しんでいってね。",
                speakerSprite = settings.otohimeFaceJoy
            },
            new DialogueNode
            {
                speakerName = " ",
                text = "たろうは宴を楽しんだ。",
                speakerSprite = null
            }
        };

        Debug.Log("BanquetEvent: Setting up dialogue with " + nodes.Count + " nodes.");
        if (dialogueTrigger != null)
        {
            dialogueTrigger.SetDialogueNodes(nodes);
        }
        else
        {
            Debug.LogWarning("BanquetEvent: DialogueTriggerコンポーネントが見つかりません。");
        }
    }
}
