using System.Collections;
using UnityEngine;

public class HealItemDropManager : MonoBehaviour
{
    public static HealItemDropManager Instance { get; private set; }

    [SerializeField] private float dropInterval = 0.1f;
    [SerializeField] private float itemLifetime = 5.0f;
    private GameObject healItemPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        // シーンを跨いで利用する場合は以下のコメントアウトを外してください
        // DontDestroyOnLoad(gameObject);

        // Resourcesフォルダー内から"HealItem"という名前のプレハブを取得
        healItemPrefab = Resources.Load<GameObject>("HealItem");
        if (healItemPrefab == null)
        {
            Debug.LogWarning("HealItem prefab not found in Resources folder.");
        }
    }

    public void DropItems(Vector3 position, int count, float scatterForce)
    {
        if (healItemPrefab == null || count <= 0) return;
        
        StartCoroutine(DropItemsCoroutine(position, count, scatterForce));
    }

    public void DropItemsAtOnce(Vector3 position, int count, float scatterForce)
    {
        if (healItemPrefab == null || count <= 0) return;

        for (int i = 0; i < count; i++)
        {
            // アイテムを生成
            GameObject item = Instantiate(healItemPrefab, position, Quaternion.identity);

            // Rigidbody2Dがあれば散らばらせる
            Rigidbody2D itemRb = item.GetComponent<Rigidbody2D>();
            if (itemRb != null)
            {
                // ランダムな方向（上方向が強め）を計算
                Vector2 randomDir = Random.insideUnitCircle.normalized;
                randomDir.y = Mathf.Abs(randomDir.y) + 0.5f; // 上方向に補正
                randomDir.Normalize();

                itemRb.AddForce(randomDir * scatterForce, ForceMode2D.Impulse);
            }
            
            // 一定時間後にアイテムを削除する
            Destroy(item, itemLifetime);
        }
    }

    private IEnumerator DropItemsCoroutine(Vector3 position, int count, float scatterForce)
    {
        for (int i = 0; i < count; i++)
        {
            // アイテムを生成
            GameObject item = Instantiate(healItemPrefab, position, Quaternion.identity);

            // Rigidbody2Dがあれば散らばらせる
            Rigidbody2D itemRb = item.GetComponent<Rigidbody2D>();
            if (itemRb != null)
            {
                // ランダムな方向（上方向が強め）を計算
                Vector2 randomDir = Random.insideUnitCircle.normalized;
                randomDir.y = Mathf.Abs(randomDir.y) + 0.5f; // 上方向に補正
                randomDir.Normalize();

                itemRb.AddForce(randomDir * scatterForce, ForceMode2D.Impulse);
            }

            // 一定時間後にアイテムを削除する
            Destroy(item, itemLifetime);

            // 指定した秒数（デフォルトは0.1秒）待機
            yield return new WaitForSeconds(dropInterval);
        }
    }
}
