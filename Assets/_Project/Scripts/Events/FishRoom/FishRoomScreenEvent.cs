using UnityEngine;

/// <summary>
/// 魚部屋画面で魚UIの表示状態を管理するイベントクラス
/// </summary>
public class FishRoomScreenEvent : MonoBehaviour
{
    [Tooltip("魚UIを表示するかどうか")]
    [SerializeField] private bool showFishUI = true;

    private GameScreen gameScreen;

    private void Awake()
    {
        // GameScreenを取得（同じオブジェクトにアタッチされている想定）
        gameScreen = GetComponent<GameScreen>();
        if (gameScreen != null)
        {
            gameScreen.onScreenLoadedEvent += OnScreenLoaded;
            gameScreen.onScreenUnloadedEvent += OnScreenUnloaded;
        }
        else
        {
            // GameScreenがなければ即時実行
            UpdateFishUIActive();
        }
    }

    private void OnDestroy()
    {
        if (gameScreen != null)
        {
            gameScreen.onScreenLoadedEvent -= OnScreenLoaded;
            gameScreen.onScreenUnloadedEvent -= OnScreenUnloaded;
        }
    }
    private void OnScreenUnloaded()
    {
        // 部屋から出たときは必ず非表示にする
        if (UIManager.Instance != null)
        {
            UIManager.Instance.Hide<FishCountUI>();
        }
    }

    private void OnEnable()
    {
        // シーン再有効化時にも状態を更新
        UpdateFishUIActive();
    }

    private void OnScreenLoaded()
    {
        UpdateFishUIActive();
    }

    /// <summary>
    /// 魚UIの表示状態を切り替える
    /// </summary>
    private void UpdateFishUIActive()
    {
        if (showFishUI && UIManager.Instance != null)
        {
            UIManager.Instance.Show<FishCountUI>();
        }
        else if (UIManager.Instance != null)
        {
            UIManager.Instance.Hide<FishCountUI>();
        }
    }
}
