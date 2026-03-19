using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        Title,
        InGame,
        Paused,
        GameOver,
        GameClear
    }

    public GameState CurrentState { get; private set; }

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
                // スタートボタンが押されたらロード専用のメニューを開く
                titleUI.Initialize(() =>
                {
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
}
