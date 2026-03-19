using System;
using System.Collections;
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
                Time.timeScale = 0f;
                if (PlayerController.Instance != null) PlayerController.Instance.SetControlEnabled(false, PlayerController.ControlPriority.System, this);
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
    /// セーブデータからロードしてゲームを復帰させます
    /// </summary>
    /// <param name="slotNumber">ロードするスロット番号</param>
    /// <param name="locationName">セーブされた場所の名称</param>
    public void LoadGameFromSave(int slotNumber, string locationName)
    {
        Debug.Log($"[GameManager] スロット{slotNumber} (場所:{locationName}) のデータでゲームを再開します。");
        
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
