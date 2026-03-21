using UnityEngine;

/// <summary>
/// センター画面でサイエンティストの表示を管理するクラス
/// </summary>
public class CenterScreenEvent : MonoBehaviour
{
    [Tooltip("サイエンティストのGameObject")]
    [SerializeField] private GameObject scientistObject;
    private GameScreen gameScreen;

    private void Awake()
    {
        // gameScreenがインスペクターからアサインされていない場合は自分自身から取得を試みる
        if (gameScreen == null)
        {
            gameScreen = GetComponent<GameScreen>();
            if (gameScreen != null)
            {
                gameScreen.onScreenLoadedEvent += OnScreenLoaded;
            }
        }
    }

    private void OnScreenLoaded()
    {
        if (scientistObject != null && FlagManager.Instance != null)
        {
            bool hasTalkedToScientist = FlagManager.Instance.GetFlag(FlagManager.FlagKey.HasTalkedToScientist.ToString(), false);
            scientistObject.SetActive(!hasTalkedToScientist);
        }
    }
}
