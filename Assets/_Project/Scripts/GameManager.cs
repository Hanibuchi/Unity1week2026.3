using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        None = -1,
        Title,
        InGame,
        Paused,
        GameOver,
        GameClear
    }

    public GameState CurrentState { get; private set; } = GameState.None;

    // ステートが変更されたときに呼ばれるイベント
    public event Action<GameState> OnGameStateChanged;

    [Header("Game Over Settings")]
    [SerializeField] private float gameOverWaitTime = 3.0f;
    [SerializeField] private Sprite systemSpeakerSprite;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // タイトル画面の初期設定
        if (UIManager.Instance != null)
        {
            var titleUI = UIManager.Instance.GetView<TitleUI>();
            if (titleUI != null)
            {
                // スタートボタンが押されたらタイトル画面を非表示にしてロード専用のメニューを開く
                titleUI.Initialize(() =>
                {
                    UIManager.Instance.Hide<TitleUI>();
                    if (SaveManager.Instance != null)
                    {
                        SaveManager.Instance.OpenLoadMenu();
                    }
                });
            }
        }

        // 初期ステートの設定
        ChangeState(GameState.Title);
    }

    /// <summary>
    /// ゲームのステートを変更します。
    /// </summary>
    public void ChangeState(GameState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        Debug.Log($"[GameManager] State changed to: {CurrentState}");

        OnGameStateChanged?.Invoke(CurrentState);

        switch (newState)
        {
            case GameState.Title:
                Time.timeScale = 1f;
                // タイトル画面を表示し、プレイヤー操作を無効にする
                if (UIManager.Instance != null) UIManager.Instance.Show<TitleUI>();
                if (PlayerController.Instance != null) PlayerController.Instance.SetControlEnabled(false, PlayerController.ControlPriority.System, this);
                break;
            case GameState.InGame:
                Time.timeScale = 1f;
                // プレイヤーの操作制限を解除してゲーム再開
                if (PlayerController.Instance != null) PlayerController.Instance.RemoveControlRequest(this);
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                // プレイヤー操作を無効化（ポーズ画面用）
                if (PlayerController.Instance != null) PlayerController.Instance.SetControlEnabled(false, PlayerController.ControlPriority.System, this);
                break;
            case GameState.GameOver:
                Time.timeScale = 1f; // ゲームオーバー時は時間を止めない
                if (PlayerController.Instance != null) PlayerController.Instance.SetControlEnabled(false, PlayerController.ControlPriority.System, this);
                StartCoroutine(GameOverSequence());
                break;
            case GameState.GameClear:
                Time.timeScale = 0f;
                if (PlayerController.Instance != null) PlayerController.Instance.SetControlEnabled(false, PlayerController.ControlPriority.System, this);
                StartCoroutine(GameClearSequence());
                break;
        }
    }

    /// <summary>
    /// ゲームを一時停止します
    /// </summary>
    public void PauseGame()
    {
        if (CurrentState == GameState.InGame)
        {
            ChangeState(GameState.Paused);
        }
    }

    /// <summary>
    /// ゲームを再開します
    /// </summary>
    public void ResumeGame()
    {
        if (CurrentState == GameState.Paused)
        {
            ChangeState(GameState.InGame);
        }
    }

    /// <summary>
    /// ゲームオーバー時の進行処理（一定時間待機→ダイアログ→ロード画面）
    /// </summary>
    private IEnumerator GameOverSequence()
    {
        // 破片などが飛び散るのを一定時間見せる
        yield return new WaitForSeconds(gameOverWaitTime);

        // プレイヤーが最後に触れたGameScreenをアンロード
        var lastGameScreen = PlayerGameScreenTracker.Instance != null ? PlayerGameScreenTracker.Instance.GetLastTouchedGameScreen() : null;
        if (lastGameScreen != null)
        {
            lastGameScreen.OnScreenUnloaded();
        }

        // ゲームオーバーダイアログの設定
        var gameOverNode = new DialogueNode
        {
            speakerSprite = systemSpeakerSprite,
            speakerName = "システム",
            text = "死んでしまった...",
            hasChoices = false
        };

        // ダイアログを表示し、終了後にロードメニューを開く
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(new List<DialogueNode> { gameOverNode }, () =>
            {
                if (SaveManager.Instance != null)
                {
                    SaveManager.Instance.OpenLoadMenu();
                }
            });
        }
        else
        {
            // DialogueManagerがないフォールバック
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.OpenLoadMenu();
            }
        }
    }

    [SerializeField] private GameScreen wildTurtleGameScreen;

    /// <summary>
    /// ゲームクリア時の進行処理（エンディング演出やダイアログなど）
    /// </summary>
    private IEnumerator GameClearSequence()
    {
        // EndingUIを表示
        if (UIManager.Instance != null)
        {
            UIManager.Instance.Show<EndingUI>();
        }

        // 2秒待機（演出用）
        yield return new WaitForSecondsRealtime(2.0f);


        // プレイヤーが最後に触れたGameScreenをアンロード
        var lastGameScreen = PlayerGameScreenTracker.Instance != null ? PlayerGameScreenTracker.Instance.GetLastTouchedGameScreen() : null;
        if (lastGameScreen != null)
        {
            lastGameScreen.OnScreenUnloaded();
        }

        // フラグの状態を取得
        bool foodWarehouse = false;
        bool vaccine = false;
        bool chemicalPlant = false;
        if (FlagManager.Instance != null)
        {
            foodWarehouse = FlagManager.Instance.GetFlag(FlagManager.FlagKey.FoodWarehouseMissionFinished.ToString());
            vaccine = FlagManager.Instance.GetFlag(FlagManager.FlagKey.VaccineMissionFinished.ToString());
            chemicalPlant = FlagManager.Instance.GetFlag(FlagManager.FlagKey.ChemicalPlantMissionFinished.ToString());
        }

        // ダイアログ終了後にロードメニューを開くコールバック
        Action onDialogueEnd = () =>
        {
            Debug.Log("[GameManager] エンディングダイアログ終了");
            StartCoroutine(HandleEndingUICloseAndLoadMenu());
        };

        // EndingUIを非表示・ロードメニューを開く処理を遅延実行するコルーチン
        IEnumerator HandleEndingUICloseAndLoadMenu()
        {
            yield return new WaitForSecondsRealtime(2.0f); // 2秒待機（必要に応じて調整）
            if (UIManager.Instance != null)
            {
                UIManager.Instance.Hide<EndingUI>();
            }
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.OpenLoadMenu();
            }
        }

        if (foodWarehouse && vaccine && chemicalPlant)
        {
            // すべて達成
            var gameClearNode = new DialogueNode
            {
                speakerSprite = systemSpeakerSprite,
                speakerName = "システム",
                text = "全てのミッションを達成し、真のエンディングを迎えました！おめでとうございます。",
                hasChoices = false
            };
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartDialogue(new List<DialogueNode> { gameClearNode }, onDialogueEnd);
            }
            else
            {
                Debug.Log("[GameManager] DialogueManagerが見つかりません。エンディングダイアログをスキップします。");
                onDialogueEnd();
            }
        }
        else
        {
            // いずれか未達成時のエンディング（複数ノードで演出、アイコン付き）
            var settings = CommonGameSettings.Settings ?? Resources.Load<GameSettingsData>("GameSettings");
            Sprite taroFace = settings != null ? settings.taroFaceSadness : null;
            Sprite narrationFace = null;
            var nodes = new List<DialogueNode>
            {
                new DialogueNode
                {
                    speakerName = "",
                    speakerSprite = narrationFace,
                    text = "数週間後。"
                },
                new DialogueNode
                {
                    speakerName = "うらしまたろう",
                    speakerSprite = taroFace,
                    text = "おとひめ様……おら、もう、動けねぇだ……"
                },
                new DialogueNode
                {
                    speakerName = "うらしまたろう",
                    speakerSprite = taroFace,
                    text = "おとひめ様……言ってたっぺな……。『絶望』か『希望』を選んだ時に……開けろって……"
                },
                new DialogueNode
                {
                    speakerName = "うらしまたろう",
                    speakerSprite = taroFace,
                    text = "おらには……もう『希望』なんて……わがんねぇだ。これでおしまいにするっぺ……"
                },
                new DialogueNode
                {
                    speakerName = "",
                    speakerSprite = narrationFace,
                    text = "パカッ、と乾いた音を立てて開いた箱から、禍々しいほどの煙が舞う。\n鏡に映る暇もなく、その肌は枯れ木のように乾き、髪は真っ白に染まった。"
                },
                new DialogueNode
                {
                    speakerName = "うらしまたろう",
                    speakerSprite = settings != null ? settings.taroFaceOld : null,
                    text = "あぁ……海が……懐かしい……だ……"
                },
                new DialogueNode
                {
                    speakerName = "",
                    speakerSprite = narrationFace,
                    text = "浦島太郎の旅は、誰も救えぬまま静かに幕を閉じた。"
                },
                new DialogueNode
                {
                    speakerName = "",
                    speakerSprite = narrationFace,
                    text = "エンディング１：人類絶滅"
                },
            };
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartDialogue(nodes, onDialogueEnd);
            }
            else
            {
                Debug.Log("[GameManager] DialogueManagerが見つかりません。エンディングダイアログをスキップします。");
                onDialogueEnd();
            }
        }
    }
    /// <summary>
    /// セーブデータからロードしてゲームを復帰させます
    /// </summary>
    /// <param name="slotNumber">ロードするスロット番号</param>
    /// <param name="locationName">セーブされた場所の名称</param>
    public void LoadGameFromSave(int slotNumber, string locationName)
    {
        Debug.Log($"[GameManager] スロット{slotNumber} (場所:{locationName}) のデータでゲームを再開します。");

        // 魚カウンターのリセット
        if (FishCountManager.Instance != null)
        {
            FishCountManager.Instance.ResetCount();
        }

        // プレイヤー能力のロードと適用
        if (PlayerAbilityManager.Instance != null)
        {
            PlayerAbilityManager.Instance.LoadAndApplyAbilities();
        }
        else
        {
            Debug.LogWarning("[GameManager] PlayerAbilityManagerが見つかりません。能力のロードをスキップします。");
        }

        // TODO: ここにシーン遷移、プレイヤー座標の復元、フェードUIなどの処理を記述します。

        if (SaveTriggerManager.Instance != null)
        {
            var savePoint = SaveTriggerManager.Instance.GetSaveTriggerByLocationName(locationName);
            if (savePoint != null && PlayerController.Instance != null)
            {
                // ゲームオーバーで非アクティブになっている場合を考慮してアクティブに戻す
                PlayerController.Instance.gameObject.SetActive(true);

                // 体力などの再初期化を行う
                var playerHealth = PlayerController.Instance.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.InitializeHealth();
                }

                // Z座標はプレイヤーのものを維持するために、XとYのみ更新します（Zズレによる消失防止）
                Vector3 newPos = savePoint.transform.position;
                newPos.z = PlayerController.Instance.transform.position.z;
                PlayerController.Instance.transform.position = newPos;

                // すでにRigidBodyの移動処理などが噛み合わない場合を考慮して速度もリセット
                var rb = PlayerController.Instance.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                }

                // セーブポイントが属するGameScreenをロードする
                if (savePoint.TargetGameScreen != null)
                {
                    savePoint.TargetGameScreen.OnScreenLoaded();
                }

                Debug.Log($"[GameManager] Player moved to {locationName} at {newPos}");
            }
            else
            {
                Debug.LogWarning($"[GameManager] 指定されたセーブポイント ({locationName}) が見つからないか、PlayerControllerがありません。");
            }
        }
        else
        {
            Debug.LogWarning("[GameManager] SaveTriggerManagerが見つかりません。プレイヤー位置の復元をスキップします。");
        }

        ChangeState(GameState.InGame);
    }
}
