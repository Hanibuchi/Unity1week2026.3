using UnityEngine;

/// <summary>
/// セーブシステム全体を管理し、UI（SaveUIView）とデータ書き込み（FlagManager）を仲介するクラスです。
/// </summary>
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    // UIを開いたときの動作モード
    public enum Mode { LoadOnly, SaveAndLoad }
    public Mode CurrentMode { get; private set; }

    [Header("Game Data")]
    [Tooltip("現在のプレイ時間（秒）")]
    public float CurrentPlayTime;

    [Tooltip("現在のセーブ場所（UI表示用）")]
    public string CurrentLocation = "---";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // プレイ時間を毎フレーム加算
        CurrentPlayTime += Time.deltaTime;
    }

    /// <summary>
    /// セーブとロードの両方が可能なメニューを開きます
    /// </summary>
    /// <param name="locationName">保存する場所の名称</param>
    public void OpenSaveAndLoadMenu(string locationName)
    {
        CurrentMode = Mode.SaveAndLoad;
        CurrentLocation = locationName;
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.Show<SaveUIView>();
        }
        else
        {
            Debug.LogWarning("UIManagerが存在しません。");
        }
    }

    /// <summary>
    /// ロード専用のメニューを開きます
    /// </summary>
    public void OpenLoadMenu()
    {
        CurrentMode = Mode.LoadOnly;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.Show<SaveUIView>();
        }
        else
        {
            Debug.LogWarning("UIManagerが存在しません。");
        }
    }

    /// <summary>
    /// SaveSlotなどのUI側でスロットがクリック・選択されたときに呼び出されるメソッド
    /// </summary>
    /// <param name="slotNumber">スロット番号(1〜)</param>
    public void OnSlotSelected(int slotNumber)
    {
        if (CurrentMode == Mode.SaveAndLoad)
        {
            // TODO: セーブかロードかを選択するUIや処理を呼び出す
            Debug.Log($"スロット{slotNumber}が選択されました。セーブするかロードするかを選ぶ処理が必要です。");
        }
        else if (CurrentMode == Mode.LoadOnly)
        {
            // データが存在する場合のみロード
            if (FlagManager.HasSaveData(slotNumber))
            {
                PerformLoad(slotNumber);

                // ロード完了後にUIを閉じる
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.Hide<SaveUIView>();
                }
            }
            else
            {
                Debug.Log($"スロット{slotNumber}にはセーブデータがありません。");
            }
        }
    }

    private void PerformSave(int slotNumber)
    {
        if (FlagManager.Instance != null)
        {
            // フラグの保存
            FlagManager.Instance.Save(slotNumber);

            // プレイ時間と場所の保存
            PlayerPrefs.SetFloat($"Slot_{slotNumber}_PlayTime", CurrentPlayTime);
            PlayerPrefs.SetString($"Slot_{slotNumber}_Location", CurrentLocation);

            // PlayerPrefsの変更を確実に書き込む
            PlayerPrefs.Save();

            Debug.Log($"スロット{slotNumber}へセーブ処理を実行しました。（時間:{GetFormattedPlayTime(slotNumber)} 場所:{CurrentLocation}）");
        }
    }

    private void PerformLoad(int slotNumber)
    {
        if (FlagManager.Instance != null)
        {
            // フラグの復元
            FlagManager.Instance.Load(slotNumber);

            // プレイ時間と場所の復元
            CurrentPlayTime = PlayerPrefs.GetFloat($"Slot_{slotNumber}_PlayTime", 0f);
            CurrentLocation = PlayerPrefs.GetString($"Slot_{slotNumber}_Location", "---");

            Debug.Log($"スロット{slotNumber}からロード処理を実行しました。（時間:{GetFormattedPlayTime(slotNumber)} 場所:{CurrentLocation}）");
            // ※ここで実際のシーン遷移や復帰イベントを呼び出す必要があります
        }
    }

    // --- UI表示用のユーティリティメソッド ---

    /// <summary>
    /// セーブされた場所の名称を取得します
    /// </summary>
    public static string GetSavedLocation(int slotNumber)
    {
        return PlayerPrefs.GetString($"Slot_{slotNumber}_Location", "---");
    }

    /// <summary>
    /// セーブされたプレイ時間をフォーマットされた文字列で取得します
    /// </summary>
    public static string GetFormattedPlayTime(int slotNumber)
    {
        float timeSeconds = PlayerPrefs.GetFloat($"Slot_{slotNumber}_PlayTime", 0f);

        int hours = Mathf.FloorToInt(timeSeconds / 3600f);
        int minutes = Mathf.FloorToInt((timeSeconds % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(timeSeconds % 60f);

        if (hours > 0)
        {
            return $"{hours}時間{minutes:00}分{seconds:00}秒";
        }
        else
        {
            return $"{minutes}分{seconds:00}秒";
        }
    }

    public string testLocation = "テストの場所";
    public void TestOpenSaveAndLoadMenu()
    {
        OpenSaveAndLoadMenu(testLocation);
    }
    public void TestOpenLoadOnlyMenu()
    {
        OpenLoadMenu();
    }
}
