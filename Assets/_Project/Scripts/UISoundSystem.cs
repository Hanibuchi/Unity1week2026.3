using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

// 2. Selectable (UI) にアタッチされ、各種イベントで音を鳴らすコンポーネント
public class UISoundEmitter : MonoBehaviour, ISelectHandler, IPointerClickHandler, ISubmitHandler
{
    private Selectable _selectable;

    private void Awake()
    {
        _selectable = GetComponent<Selectable>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (_selectable != null && _selectable.interactable)
        {
            if (CommonGameSettings.Settings != null && CommonGameSettings.Settings.uiSelectSound != null && SoundManager.Instance != null)
                SoundManager.Instance.PlaySE(CommonGameSettings.Settings.uiSelectSound);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_selectable != null && _selectable.interactable)
        {
            if (CommonGameSettings.Settings != null && CommonGameSettings.Settings.uiDecideSound != null && SoundManager.Instance != null)
                SoundManager.Instance.PlaySE(CommonGameSettings.Settings.uiDecideSound);
        }
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if (_selectable != null && _selectable.interactable)
        {
            if (CommonGameSettings.Settings != null && CommonGameSettings.Settings.uiDecideSound != null && SoundManager.Instance != null)
                SoundManager.Instance.PlaySE(CommonGameSettings.Settings.uiDecideSound);
        }
    }
}

// 3. 既存のあらゆるUI（Button, Toggle, Slider等）に上記Emitterを自動付与するシステム
public static class UIGlobalSoundAutoAssigner
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        AssignSoundsToCurrentSceneUI();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignSoundsToCurrentSceneUI();
    }

    public static void AssignSoundsToCurrentSceneUI()
    {
        var selectables = Resources.FindObjectsOfTypeAll<Selectable>();

        foreach (var selectable in selectables)
        {
            if (selectable.gameObject.scene.buildIndex == -1) continue;

            if (selectable.GetComponent<UISoundEmitter>() == null)
            {
                selectable.gameObject.AddComponent<UISoundEmitter>();
            }
        }
    }
}
