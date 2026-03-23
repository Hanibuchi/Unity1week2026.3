using UnityEngine;

public class FoodWarehouseScreenEvent : MonoBehaviour
{
    [Tooltip("生成するRubbleのプレハブ")]
    [SerializeField] private GameObject rubblePrefab;
    [Tooltip("Rubbleを生成する位置")]
    [SerializeField] private Transform rubbleSpawnPoint;

    private GameObject spawnedRubble;
    private GameScreen gameScreen;

    private void Awake()
    {
        // アタッチされているオブジェクトからGameScreenを取得
        gameScreen = GetComponent<GameScreen>();
        if (gameScreen == null)
        {
            Debug.LogError("GameScreenが見つかりません。FoodWarehouseScreenEventはGameScreenと同じオブジェクトにアタッチしてください。");
            enabled = false;
            return;
        }

        // イベントに処理を追加
        gameScreen.onScreenLoadedEvent += OnScreenLoaded;
        gameScreen.onScreenUnloadedEvent += OnScreenUnloaded;
    }

    private void OnDestroy()
    {
        if (gameScreen != null)
        {
            gameScreen.onScreenLoadedEvent -= OnScreenLoaded;
            gameScreen.onScreenUnloadedEvent -= OnScreenUnloaded;
        }
    }

    private void OnScreenLoaded()
    {
        // FoodWarehouseMissionRewardAvailable または FoodWarehouseMissionFinished フラグが立っている場合は生成しない
        if (FlagManager.Instance != null)
        {
            bool reward = FlagManager.Instance.GetFlag(FlagManager.FlagKey.FoodWarehouseMissionRewardAvailable.ToString());
            bool finished = FlagManager.Instance.GetFlag(FlagManager.FlagKey.FoodWarehouseMissionFinished.ToString());
            if (reward || finished) return;
        }
        // Rubbleを生成
        if (rubblePrefab != null && rubbleSpawnPoint != null && spawnedRubble == null)
        {
            spawnedRubble = Instantiate(rubblePrefab, rubbleSpawnPoint.position, rubbleSpawnPoint.rotation, transform);
        }
    }

    private void OnScreenUnloaded()
    {
        // Rubbleを削除
        if (spawnedRubble != null)
        {
            Destroy(spawnedRubble);
            spawnedRubble = null;
        }
    }
}
