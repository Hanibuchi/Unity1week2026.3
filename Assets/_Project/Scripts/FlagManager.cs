using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ゲーム内の進行フラグなどを管理するクラス
/// PlayerPrefsを利用してセーブ・ロードを行います。
/// </summary>
public class FlagManager : MonoBehaviour
{
    public enum FlagKey
    {
        HasVisitedFuture,
        HasBanquetEnded, // 宴が終わったかどうか

        // ペットミッション
        PetMissionRewardAvailable,   // ペットミッション完了
        PetMissionFinished,    // ペットミッション達成

        // 暴走ウミガメミッション
        WildTurtleMissionStarted, // 暴走ウミガメミッション受諾
        WildTurtleMissionRewardAvailable, // 暴走ウミガメミッション完了
        WildTurtleMissionFinished,  // 暴走ウミガメミッション達成

        // 食材確保ミッション
        IngredientMissionStarted,   // 食材確保ミッション開始
        IngredientMissionRewardAvailable, // 食材確保ミッション完了
        IngredientMissionFinished,   // 食材確保ミッション達成

        HasJetDash,   // ジェットダッシュを持っているか
        HasAttackDown,   // 攻撃ダウンを持っているか
        HasIncreaseAttack,   // 攻撃力アップを持っているか

        HasTalkedToScientist, // 科学者と話したか
    }

    public static FlagManager Instance { get; private set; }

    // 現在のフラグを保持する辞書
    private Dictionary<string, bool> currentFlags = new Dictionary<string, bool>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public bool testFlagValue = true;
    public void TestSetFlag(string key)
    {
        SetFlag(key, testFlagValue);
    }
    /// <summary>
    /// フラグを設定します
    /// </summary>
    public void SetFlag(string key, bool value)
    {
        currentFlags[key] = value;
    }

    /// <summary>
    /// フラグを取得します（設定されていない場合はdefaultValueを返します）
    /// </summary>
    public bool GetFlag(string key, bool defaultValue = false)
    {
        if (currentFlags.TryGetValue(key, out bool val))
        {
            return val;
        }
        return defaultValue;
    }

    /// <summary>
    /// 現在のフラグ状態を指定したスロットにセーブします
    /// </summary>
    public void Save(int slotIndex)
    {
        string pfx = $"Slot_{slotIndex}_Flag_";

        // 保存するキーの一覧を保持しておく（ロード用）
        List<string> savedKeys = new List<string>(currentFlags.Keys);
        string keysJson = JsonUtility.ToJson(new Serialization<string>(savedKeys));
        PlayerPrefs.SetString($"Slot_{slotIndex}_FlagKeys", keysJson);

        // 各フラグを保存
        foreach (var kvp in currentFlags)
        {
            PlayerPrefs.SetInt(pfx + kvp.Key, kvp.Value ? 1 : 0);
        }

        // セーブデータが存在することを示すフラグも保存（UI用）
        PlayerPrefs.SetInt($"Slot_{slotIndex}_HasData", 1);
        PlayerPrefs.Save();

        Debug.Log($"[FlagManager] スロット{slotIndex}にセーブしました。");
    }

    /// <summary>
    /// 指定したスロットのセーブデータをロードします
    /// </summary>
    public void Load(int slotIndex)
    {
        currentFlags.Clear();
        string pfx = $"Slot_{slotIndex}_Flag_";

        string keysJson = PlayerPrefs.GetString($"Slot_{slotIndex}_FlagKeys", string.Empty);
        if (!string.IsNullOrEmpty(keysJson))
        {
            var savedKeys = JsonUtility.FromJson<Serialization<string>>(keysJson).target;
            foreach (string key in savedKeys)
            {
                currentFlags[key] = PlayerPrefs.GetInt(pfx + key, 0) == 1;
            }
        }

        Debug.Log($"[FlagManager] スロット{slotIndex}からロードしました。");
    }

    /// <summary>
    /// 指定したスロットのデータが存在するか確認します
    /// </summary>
    public static bool HasSaveData(int slotIndex)
    {
        return PlayerPrefs.GetInt($"Slot_{slotIndex}_HasData", 0) == 1;
    }

    // ListなどのJsonUtilityシリアライズ用ヘルパークラス
    [System.Serializable]
    private class Serialization<T>
    {
        public List<T> target;
        public Serialization(List<T> target) { this.target = target; }
    }

    // public string testKey = "TestFlag";
    // public int testSlot = 1;
    // public void TestSet()
    // {
    //     SetFlag(testKey, true);
    //     Debug.Log($"[FlagManager] {testKey}をtrueに設定しました。");
    // }
    // public void TestGet()
    // {
    //     bool value = GetFlag(testKey);
    //     Debug.Log($"[FlagManager] {testKey}の値: {value}");
    // }
    // public void TestSave()
    // {
    //     Save(testSlot);
    // }
    // public void TestLoad()
    // {
    //     Load(testSlot);
    // }
}
