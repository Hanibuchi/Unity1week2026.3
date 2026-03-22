using UnityEngine;

public class WallActivatorOnPlayerSense : MonoBehaviour
{
    [Header("感知でActive化する壁オブジェクト")]
    [SerializeField] private GameObject wallA;
    [SerializeField] private GameObject wallB;

    [Header("敵（子オブジェクト）")]
    [SerializeField] private GameObject enemyChild;

    private bool playerInRange = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            if (wallA != null) wallA.SetActive(true);
            if (wallB != null) wallB.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void Update()
    {
        // 敵子オブジェクトがDestroyされたら壁を非Active化
        if (playerInRange && enemyChild == null)
        {
            if (wallA != null) wallA.SetActive(false);
            if (wallB != null) wallB.SetActive(false);
            if (wallB != null) gameObject.SetActive(false);
            playerInRange = false; // 一度だけ実行
        }
    }
}
