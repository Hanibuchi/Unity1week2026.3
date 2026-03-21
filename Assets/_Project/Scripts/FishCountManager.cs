using UnityEngine;

/// <summary>
/// 魚の切り身の数を管理するシングルトンクラス
/// </summary>
public class FishCountManager : MonoBehaviour
{
    public static FishCountManager Instance { get; private set; }

    private int _fishCount = 0;
    public int FishCount => _fishCount;
    [SerializeField] private GameScreen gameScreen;

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
        }
    }

    /// <summary>
    /// 魚の切り身の数を加算します。
    /// </summary>
    public void AddFish(int count)
    {
        _fishCount += count;

        // 20以上になったらフラグを立てる
        if (_fishCount >= 20 && FlagManager.Instance != null)
        {
            FlagManager.Instance.SetFlag(FlagManager.FlagKey.IngredientMissionRewardAvailable.ToString(), true);
            gameScreen?.OnScreenLoaded();
        }
    }

    /// <summary>
    /// 魚の切り身の数をリセットします。
    /// </summary>
    public void ResetCount()
    {
        _fishCount = 0;
    }
}
