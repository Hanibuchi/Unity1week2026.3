using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnemySpawnData
{
    [Tooltip("スポーンさせる敵のプレハブ")]
    public GameObject enemyPrefab;
    [Tooltip("スポーンする位置の基準。子オブジェクトを持たせている場合は、それらすべての子オブジェクトの位置にスポーンさせます。")]
    public Transform spawnPoint;
}

/// <summary>
/// 1つの画面（エリア）の情報を管理するクラス
/// </summary>
public class GameScreen : MonoBehaviour
{
    [Header("Camera Settings")]
    [Tooltip("この画面でのカメラの映す範囲を制限するCollider (PolygonCollider2Dなどを指定)")]
    [SerializeField] private Collider2D cameraBoundingShape;

    [Header("Enemy Spawning")]
    [Tooltip("画面ロード時にスポーンさせる敵のリスト")]
    [SerializeField] private List<EnemySpawnData> enemySpawnDataList = new List<EnemySpawnData>();

    // スポーンした敵を保持しておくリスト（画面を離れた時に破棄したい場合などに使える）
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    /// <summary>
    /// この画面に遷移した（ロードされた）時に呼ばれる
    /// </summary>
    public void OnScreenLoaded()
    {
        // 1. カメラの映す範囲をこの画面用のColliderに設定する
        if (cameraBoundingShape != null)
        {
            if (CameraController.Instance != null)
            {
                CameraController.Instance.SetBoundingShape(cameraBoundingShape);
            }
            else
            {
                Debug.LogWarning("CameraControllerのInstanceが存在しません。");
            }
        }

        // 2. 敵をスポーンさせる
        SpawnEnemies();
    }

    /// <summary>
    /// この画面から離れる（アンロードされる）時に呼ばれる
    /// </summary>
    public void OnScreenUnloaded()
    {
        // 必要に応じて、スポーンした敵を破棄するなどの処理を追加
        ClearEnemies();
    }

    private void SpawnEnemies()
    {
        foreach (var data in enemySpawnDataList)
        {
            if (data.enemyPrefab != null && data.spawnPoint != null)
            {
                // 子オブジェクトを持つ場合は、その子オブジェクトすべてをスポーン位置として扱う
                if (data.spawnPoint.childCount > 0)
                {
                    foreach (Transform child in data.spawnPoint)
                    {
                        InstantiateEnemy(data.enemyPrefab, child);
                    }
                }
                else
                {
                    // 子オブジェクトがない場合は、指定されたTransform自体をスポーン位置とする
                    InstantiateEnemy(data.enemyPrefab, data.spawnPoint);
                }
            }
        }
    }

    private void InstantiateEnemy(GameObject prefab, Transform spawnTransform)
    {
        // スポーンさせて、この画面(GameScreen)の子オブジェクトとして配置する
        GameObject enemy = Instantiate(prefab, spawnTransform.position, spawnTransform.rotation, transform);
        spawnedEnemies.Add(enemy);
    }

    private void ClearEnemies()
    {
        foreach (var enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        spawnedEnemies.Clear();
    }
}
