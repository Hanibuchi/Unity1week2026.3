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
        else if (wildTurtleGameScreen != null)
        {
            // フォールバックとして従来のGameScreenをアンロード
            wildTurtleGameScreen.OnScreenUnloaded();
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
