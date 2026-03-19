using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SaveTrigger : MonoBehaviour, IInteractable
{
    [Header("Save Configuration")]
    [Tooltip("このセーブポイントの場所名")]
    [SerializeField] private string locationName = "セーブポイント";

    [Tooltip("プレイヤーが範囲に入った時に自動で開くかどうか")]
    [SerializeField] private bool autoStart = false;

    [Tooltip("プレイヤーが範囲にいる間だけ表示するオブジェクト（操作ヒントアイコンなど）")]
    [SerializeField] private GameObject indicatorObject;

    private void Start()
    {
        if (indicatorObject != null)
        {
            indicatorObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.SetInteractable(this);
            }

            if (indicatorObject != null)
            {
                indicatorObject.SetActive(true);
            }

            if (autoStart)
            {
                Interact();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.RemoveInteractable(this);
            }

            if (indicatorObject != null)
            {
                indicatorObject.SetActive(false);
            }
        }
    }

    public void Interact()
    {
        Debug.Log("SaveTrigger: Interact called");
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.OpenSaveAndLoadMenu(locationName);
        }
        else
        {
            Debug.LogWarning("SaveManagerのインスタンスが見つかりません。");
        }
    }
}
