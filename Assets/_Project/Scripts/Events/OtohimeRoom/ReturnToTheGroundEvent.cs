using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueTrigger))]
public class ReturnToTheGroundEvent : MonoBehaviour
{
    [Header("画面遷移ゲート")]
    [SerializeField] private ScreenTransitionGate screenTransitionGate;

    [Header("ダイアログ開始までの遅延秒数")]
    [SerializeField] private float dialogueStartDelay = 0.1f;

    [Header("地上イベント")]
    [SerializeField] private GroundEvent groundEvent;
    [SerializeField] GameScreen gameScreen;

    private DialogueTrigger dialogueTrigger;
    private void Awake()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();
        SetupDialogue();
        if (dialogueTrigger != null)
        {
            dialogueTrigger.onDialogueEnd += OnDialogueEnd;
        }
        else
        {
            Debug.LogWarning("ReturnToTheGroundEvent: DialogueTriggerコンポーネントが見つかりません。");
        }
        if (gameScreen != null)
        {
            gameScreen.onScreenLoadedEvent += SetActive;
        }
    }

    void SetActive()
    {
        // returnToTheGroundEventのアクティブ制御
        bool hasVisitedFuture = FlagManager.Instance.GetFlag(FlagManager.FlagKey.HasVisitedFuture.ToString(), false);
        gameObject.SetActive(hasVisitedFuture);
    }

    /// <summary>
    /// 地上に戻るイベントをスタートさせる（画面遷移とダイアログ開始）
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
                Debug.LogWarning("ReturnToTheGroundEvent: PlayerController.Instanceが見つかりません。");
            }
        }
        else
        {
            Debug.LogWarning("ReturnToTheGroundEvent: screenTransitionGateが設定されていません。");
        }
        SetActive();
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
        // yield break;
    }

    private void OnDialogueEnd()
    {
        if (groundEvent != null)
        {
            groundEvent.StartEvent();
            Debug.Log("ReturnToTheGroundEvent: GroundEvent.StartEvent()を呼び出しました。");
        }
        else
        {
            Debug.LogWarning("ReturnToTheGroundEvent: GroundEventがアサインされていません。");
        }
    }

    private void SetupDialogue()
    {
        var settings = CommonGameSettings.Settings ?? Resources.Load<GameSettingsData>("GameSettings");
        if (settings == null)
        {
            Debug.LogWarning("ReturnToTheGroundEvent: GameSettingsが見つかりません。");
            return;
        }

        var nodes = new List<DialogueNode>
        {
            new DialogueNode
            {
                speakerName = "おとひめ",
                text = "……太郎さん、どうしても行ってしまうのね。",
                speakerSprite = settings.otohimeFaceSadness
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "おとひめ様、今まで本当にありがとよ。おら、一生忘れねぇだ。",
                speakerSprite = settings.taroFaceNormal
            },
            new DialogueNode
            {
                speakerName = "おとひめ",
                text = "私の方こそ。……これは私からの贈り物よ。この「玉手箱」を持っていって。",
                speakerSprite = settings.otohimeFaceNormal
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "うわぁ、綺麗だぁ！ありがとよ！",
                speakerSprite = settings.taroFaceSurprise
            },
            new DialogueNode
            {
                speakerName = "おとひめ",
                text = "いい、太郎さん。この箱は、あなたが「絶望」か「希望」を選んだ時にだけ開けるのよ。それまでは、絶対に開けてはダメ。約束よ。",
                speakerSprite = settings.otohimeFaceSerious
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "絶望……？希望……？なんだべ、それ。",
                speakerSprite = settings.taroFaceConfusion
            },
            new DialogueNode
            {
                speakerName = "おとひめ",
                text = "ふふ、いつかわかる時が来るわ。",
                speakerSprite = settings.otohimeFaceCute
            },
            new DialogueNode
            {
                speakerName = "カメ",
                text = "さあ、太郎さん。私の背中に。……あなたのその勇敢さが、あの大地で通用するのか、見届けさせてもらいましょう。",
                speakerSprite = settings.kameFaceNormal
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "…？",
                speakerSprite = settings.taroFaceConfusion
            }
        };

        Debug.Log("ReturnToTheGroundEvent: Setting up dialogue with " + nodes.Count + " nodes.");
        if (dialogueTrigger != null)
        {
            dialogueTrigger.SetDialogueNodes(nodes);
        }
        else
        {
            Debug.LogWarning("ReturnToTheGroundEvent: DialogueTriggerコンポーネントが見つかりません。");
        }
    }
}
