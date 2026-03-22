using UnityEngine;

/// <summary>
/// プレイヤーが最後に触れたGameScreenを記録・取得するシングルトンクラス。
/// Playerオブジェクトにアタッチして使用。
/// </summary>
public class PlayerGameScreenTracker : MonoBehaviour
{
    public static PlayerGameScreenTracker Instance { get; private set; }

    private GameScreen lastTouchedGameScreen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 2Dの場合はCollider2D、3Dの場合はCollider
        var gameScreen = other.GetComponent<GameScreen>();
        if (gameScreen != null)
        {
            lastTouchedGameScreen = gameScreen;
        }
    }

    /// <summary>
    /// 最後に触れたGameScreenを取得する
    /// </summary>
    public GameScreen GetLastTouchedGameScreen()
    {
        return lastTouchedGameScreen;
    }
}
