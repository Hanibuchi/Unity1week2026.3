using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SaveTrigger : MonoBehaviour, IInteractable
{
    [Header("Save Configuration")]
    [Tooltip("このセーブポイントの場所名")]
    [SerializeField] private string locationName = "セーブポイント";

    public string LocationName => locationName;

    [Tooltip("セーブポイントに触れた際のSE")]
    [SerializeField] private AudioClip interactSE;

    [Tooltip("セーブUIが表示されるまでの待機時間（秒）")]
    [SerializeField] private float delayBeforeUI = 1.0f;

    [Tooltip("プレイヤーが範囲に入った時に自動で開くかどうか")]
    [SerializeField] private bool autoStart = false;

    [Tooltip("プレイヤーが範囲にいる間だけ表示するオブジェクト（操作ヒントアイコンなど）")]
    [SerializeField] private GameObject indicatorObject;

    private bool isInteracting = false;

    private void OnEnable()
    {
        if (SaveTriggerManager.Instance != null)
        {
            SaveTriggerManager.Instance.Register(this);
        }
    }

    private void OnDisable()
    {
        if (SaveTriggerManager.Instance != null)
        {
            SaveTriggerManager.Instance.Unregister(this);
        }
    }

    private void Start()
    {
        if (SaveTriggerManager.Instance != null)
        {
            SaveTriggerManager.Instance.Register(this);
        }
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
        if (isInteracting) return;
        
        Debug.Log("SaveTrigger: Interact called");
        StartCoroutine(InteractCoroutine());
    }

    private System.Collections.IEnumerator InteractCoroutine()
    {
        isInteracting = true;

        if (SoundManager.Instance != null && interactSE != null)
        {
            SoundManager.Instance.PlaySE(interactSE);
        }

        if (PlayerHealth.Instance != null)
        {
            PlayerHealth.Instance.Heal(PlayerHealth.Instance.MaxHealth);
        }

        yield return new WaitForSeconds(delayBeforeUI);

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.OpenSaveAndLoadMenu(locationName);
        }
        else
        {
            Debug.LogWarning("SaveManagerのインスタンスが見つかりません。");
        }

        isInteracting = false;
    }
}
