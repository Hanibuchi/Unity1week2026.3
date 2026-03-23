using UnityEngine;

/// <summary>
/// 暴走ウミガメ部屋画面で、ボス戦イベントの表示状態を管理するイベントクラス
/// </summary>
public class WildTurtleRoomScreenEvent : MonoBehaviour
{
    [Tooltip("ボス戦イベント(WildTurtleBattleEvent)の参照")]
    [SerializeField] private WildTurtleBattleEvent wildTurtleBattleEvent;

    private GameScreen gameScreen;

    private void Awake()
    {
        // GameScreenを取得（同じオブジェクトにアタッチされている想定）
        gameScreen = GetComponent<GameScreen>();
        if (gameScreen != null)
        {
            gameScreen.onScreenLoadedEvent += OnScreenLoaded;
        }
        else
        {
            // GameScreenがなければ即時実行
            UpdateBattleEventActive();
        }
    }

    private void OnScreenLoaded()
    {
        UpdateBattleEventActive();
    }

    /// <summary>
    /// フラグに応じてボス戦イベントの表示状態を切り替える
    /// </summary>
    private void UpdateBattleEventActive()
    {
        if (wildTurtleBattleEvent == null)
        {
            Debug.LogWarning("[WildTurtleRoomScreenEvent] wildTurtleBattleEventが設定されていません。");
            return;
        }
        bool rewardAvailable = false;
        bool missionFinished = false;
        if (FlagManager.Instance != null)
        {
            rewardAvailable = FlagManager.Instance.GetFlag(FlagManager.FlagKey.WildTurtleMissionRewardAvailable.ToString(), false);
            missionFinished = FlagManager.Instance.GetFlag(FlagManager.FlagKey.WildTurtleMissionFinished.ToString(), false);
        }
        // いずれかのフラグが立っていれば非アクティブ、それ以外はアクティブ
        wildTurtleBattleEvent.gameObject.SetActive(!(rewardAvailable || missionFinished));
    }
}
