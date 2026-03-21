using UnityEngine;

/// <summary>
/// ペット部屋画面でペットオブジェクトの表示状態を管理するイベントクラス
/// </summary>
public class PetRoomScreenEvent : MonoBehaviour
{
    [Tooltip("ペットのGameObject")]
    [SerializeField] private GameObject petObject;


    private GameScreen gameScreen;

    private void Awake()
    {
        // GameScreenを取得（同じオブジェクトにアタッチされている想定）
        gameScreen = GetComponent<GameScreen>();
        if (gameScreen != null)
        {
            gameScreen.onScreenLoadedEvent += OnScreenLoaded;
        }
        else
        {
            // GameScreenがなければ即時実行
            UpdatePetObjectActive();
        }
    }

    private void OnDestroy()
    {
        if (gameScreen != null)
        {
            gameScreen.onScreenLoadedEvent -= OnScreenLoaded;
        }
    }

    private void OnEnable()
    {
        // シーン再有効化時にも状態を更新
        UpdatePetObjectActive();
    }

    private void OnScreenLoaded()
    {
        UpdatePetObjectActive();
    }

    /// <summary>
    /// ペットの表示状態をフラグに応じて切り替える
    /// </summary>
    private void UpdatePetObjectActive()
    {
        if (petObject == null)
        {
            Debug.LogWarning("[PetRoomScreenEvent] petObjectが設定されていません。");
            return;
        }
        bool rewardAvailable = false;
        bool missionFinished = false;
        if (FlagManager.Instance != null)
        {
            rewardAvailable = FlagManager.Instance.GetFlag(FlagManager.FlagKey.PetMissionRewardAvailable.ToString(), false);
            missionFinished = FlagManager.Instance.GetFlag(FlagManager.FlagKey.PetMissionFinished.ToString(), false);
        }
        // いずれかのフラグが立っていれば非アクティブ、それ以外はアクティブ
        petObject.SetActive(!(rewardAvailable || missionFinished));
    }
}
