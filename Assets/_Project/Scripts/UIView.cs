using UnityEngine;

/// <summary>
/// 全てのUIの基底となるクラス
/// </summary>
public abstract class UIView : MonoBehaviour, IUIView
{
    private bool _isVisible = false;
    public bool IsVisible => _isVisible;

    /// <summary>
    /// UIを表示する処理。必要な場合は各クラスでオーバーライドする
    /// </summary>
    public virtual void Show()
    {
        _isVisible = true;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// UIを非表示にする処理。必要な場合は各クラスでオーバーライドする
    /// </summary>
    public virtual void Hide()
    {
        _isVisible = false;
        gameObject.SetActive(false);
    }
}
