using UnityEngine;

public class WallActivatorOnPlayerSense : MonoBehaviour
{
    [Header("感知でActive化する壁オブジェクト")]
    [SerializeField] private GameObject wallA;
    [SerializeField] private GameObject wallB;

    [Header("敵（子オブジェクト）")]
    [SerializeField] private GameObject enemyChild;

    [Header("壁出現時に鳴らすSE")]
    [SerializeField] private AudioClip appearSE;

    [SerializeField] private AudioClip enemyDestroySE;

    private bool playerInRange = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            if (wallA != null) wallA.SetActive(true);
            if (wallB != null) wallB.SetActive(true);
            if (SoundManager.Instance != null && appearSE != null)
            {
                SoundManager.Instance.PlaySE(appearSE);
            }
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
            if (SoundManager.Instance != null && enemyDestroySE != null)
            {
                SoundManager.Instance.PlaySE(enemyDestroySE);
            }
            playerInRange = false; // 一度だけ実行
        }
    }
}
