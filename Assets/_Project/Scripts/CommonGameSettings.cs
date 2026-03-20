using UnityEngine;

// 1. 設定用ファイルのScriptableObject
[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettings")]
public class GameSettingsData : ScriptableObject
{
    [Header("UI Sounds")]
    public AudioClip uiSelectSound;
    public AudioClip uiDecideSound;

    [Header("Character Faces (Taro)")]
    public Sprite taroFaceNormal;
    public Sprite taroFaceJoy;
    public Sprite taroFaceAnger;
    public Sprite taroFaceSadness;
    public Sprite taroFaceConfusion;
    public Sprite taroFaceSurprise;
    public Sprite taroFaceDisgust;
    public Sprite taroFaceDamage;
    public Sprite taroFaceConfidence;

    [Header("Character Faces (Children)")]
    public Sprite childFaceConfidence;
    public Sprite childFaceSurprise;
    public Sprite childFaceDamage;

    [Header("Character Faces (Kame)")]
    public Sprite kameFaceNormal;
    public Sprite kameFaceJoy;
    public Sprite kameFaceAnger;
    public Sprite kameFaceSadness;
    public Sprite kameFaceConfusion;
    public Sprite kameFaceSurprise;
    public Sprite kameFaceDisgust;
    public Sprite kameFaceDamage;
    public Sprite kameFaceConfidence;
}

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