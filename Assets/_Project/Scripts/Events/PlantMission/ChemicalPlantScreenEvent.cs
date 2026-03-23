using UnityEngine;
using System.Collections.Generic;

public class ChemicalPlantScreenEvent : MonoBehaviour
{
    [Tooltip("生成する敵のプレハブ")]
    [SerializeField] private GameObject enemyPrefab1;
    [SerializeField] private GameObject enemyPrefab2;
    [Tooltip("敵を生成する位置（複数可。各Transformの子があれば全ての子の位置に生成）")]
    [SerializeField] private Transform enemySpawnRoots1;
    [SerializeField] private Transform enemySpawnRoots2;

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private GameScreen gameScreen;

    private void Awake()
    {
        gameScreen = GetComponent<GameScreen>();
        if (gameScreen == null)
        {
            Debug.LogError("GameScreenが見つかりません。ChemicalPlantScreenEventはGameScreenと同じオブジェクトにアタッチしてください。");
            enabled = false;
            return;
        }
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
        // ChemicalPlantMissionFinished フラグが立っている場合は生成しない
        if (FlagManager.Instance != null)
        {
            bool finished = FlagManager.Instance.GetFlag(FlagManager.FlagKey.ChemicalPlantMissionFinished.ToString());
            if (finished) return;
        }
        // 敵を生成
        if (enemyPrefab1 != null && enemySpawnRoots1 != null && spawnedEnemies.Count == 0)
        {
            if (enemySpawnRoots1.childCount > 0)
            {
                foreach (Transform child in enemySpawnRoots1)
                {
                    SpawnEnemyAt(enemyPrefab1, child);
                }
            }
            else
            {
                SpawnEnemyAt(enemyPrefab1, enemySpawnRoots1);
            }
        }
        if (enemyPrefab2 != null && enemySpawnRoots2 != null)
        {
            if (enemySpawnRoots2.childCount > 0)
            {
                foreach (Transform child in enemySpawnRoots2)
                {
                    SpawnEnemyAt(enemyPrefab2, child);
                }
            }
            else
            {
                SpawnEnemyAt(enemyPrefab2, enemySpawnRoots2);
            }
        }
    }

    private void OnScreenUnloaded()
    {
        // 生成した敵をすべて削除
        foreach (var enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        spawnedEnemies.Clear();
    }

    private void SpawnEnemyAt(GameObject enemyPrefab, Transform spawnPoint)
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation, transform);
        // spawnPointのscaleを適用
        enemy.transform.localScale = spawnPoint.localScale;
        spawnedEnemies.Add(enemy);
    }
}
