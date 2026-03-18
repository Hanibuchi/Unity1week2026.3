public interface IUIView
{
    /// <summary>
    /// UIを表示する
    /// </summary>
    void Show();

    /// <summary>
    /// UIを非表示にする
    /// </summary>
    void Hide();

    /// <summary>
    /// UIが現在表示されているかどうか
    /// </summary>
    bool IsVisible { get; }
}
