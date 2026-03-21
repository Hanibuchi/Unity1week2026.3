using UnityEngine;

/// <summary>
/// 乙姫の部屋の画面でのみ発生するイベントを管理するクラス
/// </summary>
public class OtohimeRoomScreenEvent : MonoBehaviour
{
    private GameScreen gameScreen;
    [Tooltip("宴が終わっていない場合にアクティブにするオブジェクト")]
    [SerializeField] private GameObject banquetEvent;

    private void Awake()
    {
        // gameScreenがインスペクターからアサインされていない場合は自分自身から取得を試みる
        if (gameScreen == null)
        {
            gameScreen = GetComponent<GameScreen>();
        }
    }

    private void OnEnable()
    {
        if (gameScreen != null)
        {
            gameScreen.onScreenLoadedEvent += OnScreenLoaded;
        }
        else
        {
            Debug.LogWarning("[OtohimeRoomScreenEvent] GameScreenが設定されていません。");
        }
    }

    private void OnDisable()
    {
        if (gameScreen != null)
        {
            gameScreen.onScreenLoadedEvent -= OnScreenLoaded;
        }
    }

    private void OnScreenLoaded()
    {
        if (FlagManager.Instance != null && banquetEvent != null)
        {
            // 宴が終わったかのフラグを取得（デフォルトはfalse = 宴は終わっていない）
            bool hasBanquetEnded = FlagManager.Instance.GetFlag(FlagManager.FlagKey.HasBanquetEnded.ToString(), false);
            // 宴が終わっていない(false)場合はアクティブ、終わっている(true)場合は非アクティブ
            banquetEvent.SetActive(!hasBanquetEnded);
        }
    }
}
