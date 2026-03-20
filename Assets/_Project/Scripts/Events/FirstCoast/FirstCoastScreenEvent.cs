using UnityEngine;

/// <summary>
/// 最初の海岸の画面でのみ発生するイベントを管理するクラス
/// </summary>
public class FirstCoastScreenEvent : MonoBehaviour
{
    private GameScreen gameScreen;
    [Tooltip("未来へ来たことがあるかどうかのフラグキー")]
    [SerializeField] private GameObject blockadeObject;

    [Header("Background Renderers")]
    [SerializeField] private SpriteRenderer backgroundRenderer1;
    [SerializeField] private SpriteRenderer backgroundRenderer2;

    [Header("Present Sprites (hasVisitedFuture = false)")]
    [SerializeField] private Sprite presentSprite1;
    [SerializeField] private Sprite presentSprite2;

    [Header("Future Sprites (hasVisitedFuture = true)")]
    [SerializeField] private Sprite futureSprite1;
    [SerializeField] private Sprite futureSprite2;

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
            Debug.LogWarning("[FirstCoastScreenEvent] GameScreenが設定されていません。");
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
        if (FlagManager.Instance != null && blockadeObject != null)
        {
            // 未来に来たことがあるかのフラグを取得（デフォルトはfalse）
            bool hasVisitedFuture = FlagManager.Instance.GetFlag(FlagManager.FlagKey.HasVisitedFuture.ToString(), false);

            // 未来に来たことがない(false)場合は通せん房オブジェクトをアクティブにする
            // 未来に来たことがある(true)場合は非アクティブにする
            blockadeObject.SetActive(!hasVisitedFuture);
        }

        // 背景のSpriteをフラグに応じて切り替える
        bool visitedFutureForSprite = FlagManager.Instance != null && FlagManager.Instance.GetFlag(FlagManager.FlagKey.HasVisitedFuture.ToString(), false);

        if (backgroundRenderer1 != null)
        {
            backgroundRenderer1.sprite = visitedFutureForSprite ? futureSprite1 : presentSprite1;
        }
        if (backgroundRenderer2 != null)
        {
            backgroundRenderer2.sprite = visitedFutureForSprite ? futureSprite2 : presentSprite2;
        }
    }
}
