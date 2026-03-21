using UnityEngine;

/// <summary>
/// OnDestroy時にFishCountManagerへ指定数を加算するクラス
/// </summary>
public class AddFishOnDestroy : MonoBehaviour
{
    [SerializeField] private int addCount = 1;

    private void OnDestroy()
    {
        if (FishCountManager.Instance != null)
        {
            FishCountManager.Instance.AddFish(addCount);
        }
    }
}
