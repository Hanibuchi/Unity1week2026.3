using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueTrigger))]
public class WildTurtleBattleEvent : MonoBehaviour
{
    [Header("閉じ込め用オブジェクト（会話後にアクティブ化）")]
    [SerializeField] private GameObject lockObject;
    [Header("ワイルドウミガメのプレハブ")]
    [SerializeField] private GameObject wildTurtlePrefab;
    [Header("ワイルドウミガメの出現位置")]
    [SerializeField] private Transform wildTurtleSpawnPoint;


    [Header("このイベントが発生するGameScreen")]
    [SerializeField] private GameScreen targetGameScreen;
    [Header("ボス戦開始時のSE")]
    [SerializeField] private AudioClip bossBattleStartSE;

    private DialogueTrigger dialogueTrigger;
    private GameObject spawnedWildTurtle;
    // バトル中断フラグ
    private bool isBattleAborted = false;


    private void Awake()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();
        SetupDialogue();
        if (targetGameScreen != null)
        {
            targetGameScreen.onScreenLoadedEvent += RegisterDialogueEndHandler;
            targetGameScreen.onScreenUnloadedEvent += OnScreenUnloaded;
        }
        else
        {
            Debug.LogWarning("WildTurtleEvent: targetGameScreenが設定されていません。");
        }
    }

    void OnScreenUnloaded()
    {
        if (lockObject != null)
        {
            lockObject.SetActive(false);
        }
        // バトル中断フラグを立ててからDestroy
        isBattleAborted = true;
        Destroy(spawnedWildTurtle);
    }

    private void RegisterDialogueEndHandler()
    {
        if (dialogueTrigger != null)
        {
            dialogueTrigger.onDialogueEnd = OnDialogueEnded;
        }
    }

    private void OnDialogueEnded()
    {
        // プレイヤーを閉じ込めるオブジェクトをアクティブ化
        if (lockObject != null)
        {
            lockObject.SetActive(true);
        }
        // ボス戦開始SEを再生（インスペクタから設定）
        if (bossBattleStartSE != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySE(bossBattleStartSE);
        }
        isBattleAborted = false;
        // ワイルドウミガメを生成
        if (wildTurtlePrefab != null && wildTurtleSpawnPoint != null)
        {
            spawnedWildTurtle = Instantiate(wildTurtlePrefab, wildTurtleSpawnPoint.position, wildTurtleSpawnPoint.rotation);
            // 生成したウミガメの破壊を監視
            var tracker = GroupDestroyTracker.Create(new List<GameObject> { spawnedWildTurtle });
            tracker.onAllDestroyed.AddListener(OnWildTurtleDefeated);
        }
        // 自身を非アクティブ化
        gameObject.SetActive(false);

        if (dialogueTrigger != null)
        {
            dialogueTrigger.onDialogueEnd -= OnDialogueEnded;
        }
    }

    private void OnWildTurtleDefeated()
    {
        // バトル中断時は何もしない
        if (isBattleAborted)
        {
            return;
        }
        // フラグを立てる
        FlagManager.Instance?.SetFlag(FlagManager.FlagKey.WildTurtleMissionRewardAvailable.ToString(), true);
        // 逃げていく会話を表示
        ShowWildTurtleEscapeDialogue();
        // GameScreenを再読み込み
        if (targetGameScreen != null)
        {
            targetGameScreen.OnScreenLoaded();
        }

        if (lockObject != null)
        {
            lockObject.SetActive(false);
        }
    }

    private void ShowWildTurtleEscapeDialogue()
    {
        // var settings = CommonGameSettings.Settings ?? Resources.Load<GameSettingsData>("GameSettings");
        // if (dialogueTrigger == null || settings == null) return;
        // var nodes = new List<DialogueNode>
        // {
        //     new DialogueNode
        //     {
        //         speakerName = "巨大ウミガメ",
        //         text = "くっ……今日はこのくらいにしてやる！覚えてろよ！",
        //         speakerSprite = settings.redKameFaceSurprise
        //     },
        //     new DialogueNode
        //     {
        //         speakerName = "うらしまたろう",
        //         text = "あいつ、逃げていっただな……。また来るかもしれねぇ。",
        //         speakerSprite = settings.taroFaceNormal
        //     }
        // };
        // dialogueTrigger.SetDialogueNodes(nodes);
        // dialogueTrigger.Interact();
    }

    private void SetupDialogue()
    {
        var settings = CommonGameSettings.Settings ?? Resources.Load<GameSettingsData>("GameSettings");
        if (settings == null)
        {
            Debug.LogWarning("WildTurtleEvent: GameSettingsが見つかりません。");
            return;
        }
        // 最初の会話内容をここで設定（例）
        var nodes = new List<DialogueNode>
        {
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "ここがウミガメの住処か……。なんだか嫌な予感がするだ……。",
                speakerSprite = settings.taroFaceConfusion
            },
            new DialogueNode
            {
                speakerName = "巨大ウミガメ",
                text = "グオオオオオ！ここは俺様の縄張りだ！邪魔する奴は許さねぇ！",
                speakerSprite = settings.redKameFaceAnger
            }
        };
        dialogueTrigger.SetDialogueNodes(nodes);
    }
}
