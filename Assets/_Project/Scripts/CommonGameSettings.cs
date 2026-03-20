using UnityEngine;

// 2. ヒエラルキーに配置するシングルトンクラス
// MonoBehaviourを継承したクラスは、ファイル名とクラス名が一致していないとアタッチできない場合があります。
public class CommonGameSettings : MonoBehaviour
{
    public static CommonGameSettings Instance { get; private set; }

    // 読み込んだ設定用データを保持するstaticメンバ
    public static GameSettingsData Settings { get; private set; }

    public static void Initialize()
    {
        Settings = Resources.Load<GameSettingsData>("GameSettings");
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // ResourcesフォルダからScriptableObjectを読み込む
            Initialize();
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