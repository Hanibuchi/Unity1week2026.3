using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Registered Views")]
    [Tooltip("インスペクタから各UIViewを割り当ててください")]
    [SerializeField] private List<UIView> registeredViews = new List<UIView>();

    // 内部的な辞書は型で検索できるように保持しておく
    private Dictionary<Type, IUIView> _views = new Dictionary<Type, IUIView>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeViews();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// インスペクタで設定されたUIを辞書に登録します。
    /// </summary>
    private void InitializeViews()
    {
        _views.Clear();
        foreach (var view in registeredViews)
        {
            if (view != null)
            {
                var type = view.GetType();
                if (!_views.ContainsKey(type))
                {
                    _views.Add(type, view);
                }
                else
                {
                    Debug.LogWarning($"[UIManager] View of type {type} is already registered in the Inspector.");
                }
            }
        }
    }

    /// <summary>
    /// 指定された型のUIを表示します。
    /// </summary>
    public void Show<T>() where T : class, IUIView
    {
        var view = GetView<T>();
        if (view != null)
        {
            view.Show();
        }
    }

    /// <summary>
    /// 指定された型のUIを非表示にします。
    /// </summary>
    public void Hide<T>() where T : class, IUIView
    {
        var view = GetView<T>();
        if (view != null)
        {
            view.Hide();
        }
    }

    /// <summary>
    /// 指定された型のUIインスタンスを取得します。
    /// </summary>
    public T GetView<T>() where T : class, IUIView
    {
        if (_views.TryGetValue(typeof(T), out IUIView view))
        {
            return view as T;
        }

        Debug.LogError($"[UIManager] View of type {typeof(T)} is not assigned in the UIManager Inspector.");
        return null;
    }

    // public void Test_ShowSettings()
    // {
    //     Show<SettingsUI>();
    // }
}
