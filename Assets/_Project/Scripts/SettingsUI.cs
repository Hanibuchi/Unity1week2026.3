using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;
    [SerializeField] private Button closeButton;

    private void Start()
    {
        // スライダーのイベント登録
        if (bgmSlider != null)
        {
            if (SoundManager.Instance != null)
            {
                bgmSlider.value = bgmSlider.maxValue > 0 ? SoundManager.Instance.BGMVolume * bgmSlider.maxValue : SoundManager.Instance.BGMVolume;
            }
            bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        }

        if (seSlider != null)
        {
            if (SoundManager.Instance != null)
            {
                seSlider.value = seSlider.maxValue > 0 ? SoundManager.Instance.SEVolume * seSlider.maxValue : SoundManager.Instance.SEVolume;
            }
            seSlider.onValueChanged.AddListener(OnSEVolumeChanged);
        }

        // 閉じるボタンのイベント登録
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseSettings);
        }
    }

    private void OnDestroy()
    {
        // メモリリークを防ぐためのイベント解除
        if (bgmSlider != null)
            bgmSlider.onValueChanged.RemoveListener(OnBGMVolumeChanged);
            
        if (seSlider != null)
            seSlider.onValueChanged.RemoveListener(OnSEVolumeChanged);
            
        if (closeButton != null)
            closeButton.onClick.RemoveListener(CloseSettings);
    }

    private void OnBGMVolumeChanged(float volume)
    {
        if (SoundManager.Instance != null && bgmSlider != null)
        {
            // スライダーが整数値(Whole Numbers)の場合、maxValueを使って0.0～1.0の範囲に変換
            float normalizedVolume = bgmSlider.maxValue > 0 ? volume / bgmSlider.maxValue : 0;
            SoundManager.Instance.SetBGMVolume(normalizedVolume);
        }
    }

    private void OnSEVolumeChanged(float volume)
    {
        if (SoundManager.Instance != null && seSlider != null)
        {
            // スライダーが整数値(Whole Numbers)の場合、maxValueを使って0.0～1.0の範囲に変換
            float normalizedVolume = seSlider.maxValue > 0 ? volume / seSlider.maxValue : 0;
            SoundManager.Instance.SetSEVolume(normalizedVolume);
        }
    }

    public void OpenSettings()
    {
        // UIを表示する
        gameObject.SetActive(true);
        
        // オプション: ここで SoundManager から現在の音量を取得してスライダーの値に反映すると
        // 開くたびに最新の音量がUIに表示されます。
        if (SoundManager.Instance != null)
        {
            if (bgmSlider != null)
            {
                bgmSlider.value = bgmSlider.maxValue > 0 ? SoundManager.Instance.BGMVolume * bgmSlider.maxValue : SoundManager.Instance.BGMVolume;
            }
            if (seSlider != null)
            {
                seSlider.value = seSlider.maxValue > 0 ? SoundManager.Instance.SEVolume * seSlider.maxValue : SoundManager.Instance.SEVolume;
            }
        }
    }

    private void CloseSettings()
    {
        // UIを非表示にする
        gameObject.SetActive(false);
    }
}
