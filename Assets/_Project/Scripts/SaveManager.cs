using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

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

    [Header("Dialogue UI Settings")]
    [Tooltip("システムメッセージ用（セーブ・ロード等）のスピーカー画像")]
    [SerializeField] private Sprite systemSpeakerSprite;

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
            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.SetControlEnabled(false, PlayerController.ControlPriority.UI, this);
            }
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
            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.SetControlEnabled(false, PlayerController.ControlPriority.UI, this);
            }
        }
        else
        {
            Debug.LogWarning("UIManagerが存在しません。");
        }
    }

    /// <summary>
    /// メニューを閉じる際の処理
    /// </summary>
    public void CloseMenu()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.Hide<SaveUIView>();
        }
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.RemoveControlRequest(this);
        }

        // タイトル画面でロードメニューを開いていた場合は、メニューを閉じた時にタイトル画面を再表示する
        if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameManager.GameState.Title)
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.Show<TitleUI>();
            }
        }
    }

    /// <summary>
    /// SaveSlotなどのUI側でスロットがクリック・選択されたときに呼び出されるメソッド
    /// </summary>
    /// <param name="slotNumber">スロット番号(1〜)</param>
    public void OnSlotSelected(int slotNumber)
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning("DialogueManagerが見つかりません。");
            return;
        }

        if (CurrentMode == Mode.SaveAndLoad)
        {
            CreateAndStartSaveLoadDialogue(slotNumber);
        }
        else if (CurrentMode == Mode.LoadOnly)
        {
            CreateAndStartLoadOnlyDialogue(slotNumber);
        }
    }

    private void CreateAndStartLoadOnlyDialogue(int slotNumber)
    {
        if (!FlagManager.HasSaveData(slotNumber))
        {
            // データがない場合のメッセージ
            var noDataNode = new DialogueNode
            {
                speakerSprite = systemSpeakerSprite,
                speakerName = "システム",
                text = $"スロット{slotNumber}にはセーブデータがありません。",
                hasChoices = false
            };
            StartDialogueWithLock(new List<DialogueNode> { noDataNode }, slotNumber);
            return;
        }

        var loadExecuteNode = new DialogueNode
        {
            speakerSprite = systemSpeakerSprite,
            speakerName = "システム",
            text = "ロードしました。",
            hasChoices = false,
            onNodeStart = new UnityEvent()
        };
        loadExecuteNode.onNodeStart.AddListener(() =>
        {
            PerformLoad(slotNumber);
            if (UIManager.Instance != null) UIManager.Instance.Hide<SaveUIView>();
            if (PlayerController.Instance != null) PlayerController.Instance.RemoveControlRequest(this);
        });

        var confirmNode = new DialogueNode
        {
            speakerSprite = systemSpeakerSprite,
            speakerName = "システム",
            text = $"スロット{slotNumber}をロードしますか？",
            hasChoices = true,
            choice1Text = "はい",
            choice2Text = "いいえ",
            choice1NextNodes = new List<DialogueNode> { loadExecuteNode },
            choice2NextNodes = new List<DialogueNode>() // 「いいえ」なら空リストで終了
        };

        StartDialogueWithLock(new List<DialogueNode> { confirmNode }, slotNumber);
    }

    private void CreateAndStartSaveLoadDialogue(int slotNumber)
    {
        // --- 実行ノード ---
        var saveExecuteNode = new DialogueNode
        {
            speakerSprite = systemSpeakerSprite,
            speakerName = "システム",
            text = "セーブしました。",
            hasChoices = false,
            onNodeStart = new UnityEvent()
        };
        saveExecuteNode.onNodeStart.AddListener(() =>
        {
            PerformSave(slotNumber);
            if (UIManager.Instance != null)
            {
                var saveUIView = UIManager.Instance.GetView<SaveUIView>();
                if (saveUIView != null) saveUIView.RefreshSlots();
            }
        });

        var loadExecuteNode = new DialogueNode
        {
            speakerSprite = systemSpeakerSprite,
            speakerName = "システム",
            text = "ロードしました。",
            hasChoices = false,
            onNodeStart = new UnityEvent()
        };
        loadExecuteNode.onNodeStart.AddListener(() =>
        {
            PerformLoad(slotNumber);
            if (UIManager.Instance != null) UIManager.Instance.Hide<SaveUIView>();
            if (PlayerController.Instance != null) PlayerController.Instance.RemoveControlRequest(this);
        });

        // --- 確認ノード ---
        var saveConfirmNode = new DialogueNode
        {
            speakerSprite = systemSpeakerSprite,
            speakerName = "システム",
            text = $"スロット{slotNumber}にセーブしますか？\n(上書きされます)",
            hasChoices = true,
            choice1Text = "はい",
            choice2Text = "いいえ",
            choice1NextNodes = new List<DialogueNode> { saveExecuteNode },
            choice2NextNodes = new List<DialogueNode>() // 「いいえ」なら空リストで終了
        };

        DialogueNode loadConfirmNode;
        if (FlagManager.HasSaveData(slotNumber))
        {
            loadConfirmNode = new DialogueNode
            {
                speakerSprite = systemSpeakerSprite,
                speakerName = "システム",
                text = $"スロット{slotNumber}をロードしますか？",
                hasChoices = true,
                choice1Text = "はい",
                choice2Text = "いいえ",
                choice1NextNodes = new List<DialogueNode> { loadExecuteNode },
                choice2NextNodes = new List<DialogueNode>() // 「いいえ」なら終了
            };
        }
        else
        {
            loadConfirmNode = new DialogueNode
            {
                speakerSprite = systemSpeakerSprite,
                speakerName = "システム",
                text = $"スロット{slotNumber}にはセーブデータがありません。",
                hasChoices = false
            };
        }

        // --- 最初の選択ノード ---
        var firstNode = new DialogueNode
        {
            speakerSprite = systemSpeakerSprite,
            speakerName = "システム",
            text = $"スロット{slotNumber}が選択されました。\nセーブしますか？ロードしますか？",
            hasChoices = true,
            choice1Text = "セーブ",
            choice2Text = "ロード",
            choice1NextNodes = new List<DialogueNode> { saveConfirmNode },
            choice2NextNodes = new List<DialogueNode> { loadConfirmNode }
        };

        StartDialogueWithLock(new List<DialogueNode> { firstNode }, slotNumber);
    }

    private void StartDialogueWithLock(List<DialogueNode> nodes, int slotNumber)
    {
        if (UIManager.Instance != null)
        {
            var saveUIView = UIManager.Instance.GetView<SaveUIView>();
            if (saveUIView != null)
            {
                saveUIView.SetInteractable(false);
            }
        }

        DialogueManager.Instance.StartDialogue(nodes, () => 
        {
            if (UIManager.Instance != null)
            {
                var saveUIView = UIManager.Instance.GetView<SaveUIView>();
                if (saveUIView != null)
                {
                    saveUIView.SetInteractable(true);
                    saveUIView.SelectSlot(slotNumber);
                }
            }
        });
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
