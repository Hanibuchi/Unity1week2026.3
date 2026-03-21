using UnityEngine;

/// <summary>
/// 魚の切り身の数を管理するシングルトンクラス
/// </summary>
public class FishCountManager : MonoBehaviour
{
    public static FishCountManager Instance { get; private set; }

    private int _fishCount = 0;
    public int FishCount => _fishCount;

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
    }

    /// <summary>
    /// 魚の切り身の数をリセットします。
    /// </summary>
    public void ResetCount()
    {
        _fishCount = 0;
    }
}
