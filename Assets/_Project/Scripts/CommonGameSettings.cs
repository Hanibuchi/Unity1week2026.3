using UnityEngine;

// 1. 設定用ファイルのScriptableObject
[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettings")]
public class GameSettingsData : ScriptableObject
{
    [Header("UI Sounds")]
    public AudioClip uiSelectSound;
    public AudioClip uiDecideSound;
}

// 2. ヒエラルキーに配置するシングルトンクラス
// MonoBehaviourを継承したクラスは、ファイル名とクラス名が一致していないとアタッチできない場合があります。
public class CommonGameSettings : MonoBehaviour
{
    public static CommonGameSettings Instance { get; private set; }
    
    // 読み込んだ設定用データを保持するstaticメンバ
    public static GameSettingsData Settings { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // ResourcesフォルダからScriptableObjectを読み込む
            Settings = Resources.Load<GameSettingsData>("GameSettings");
            if (Settings == null)
            {
                Debug.LogWarning("GameSettingsが見つかりません。Resourcesフォルダ配下に GameSettings という名前のファイルを作成してください。");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}