using UnityEngine;

/// <summary>
/// 乙姫の部屋の画面でのみ発生するイベントを管理するクラス
/// </summary>
public class OtohimeRoomScreenEvent : MonoBehaviour
{
    private GameScreen gameScreen;
    [Tooltip("宴が終わっていない場合にアクティブにするオブジェクト")]
    [SerializeField] private GameObject banquetEvent;
    [Tooltip("宴が終わった後にアクティブにする乙姫オブジェクト")]
    [SerializeField] private GameObject otohimeObject;

    private void Awake()
    {
        // gameScreenがインスペクターからアサインされていない場合は自分自身から取得を試みる
        if (gameScreen == null)
        {
            gameScreen = GetComponent<GameScreen>();
        }
    }

    private void OnEnable()
    {
        if (gameScreen != null)
        {
            gameScreen.onScreenLoadedEvent += OnScreenLoaded;
        }
        else
        {
            Debug.LogWarning("[OtohimeRoomScreenEvent] GameScreenが設定されていません。");
        }
    }

    private void OnDisable()
    {
        if (gameScreen != null)
        {
            gameScreen.onScreenLoadedEvent -= OnScreenLoaded;
        }
    }

    private void OnScreenLoaded()
    {
        if (FlagManager.Instance != null)
        {
            // 宴が終わったかのフラグを取得（デフォルトはfalse = 宴は終わっていない）
            bool hasBanquetEnded = FlagManager.Instance.GetFlag(FlagManager.FlagKey.HasBanquetEnded.ToString(), false);
            if (banquetEvent != null)
            {
                // 宴が終わっていない(false)場合はアクティブ、終わっている(true)場合は非アクティブ
                banquetEvent.SetActive(!hasBanquetEnded);
            }
            if (otohimeObject != null)
            {
                // 宴が終わっていた場合のみ乙姫オブジェクトをアクティブ
                otohimeObject.SetActive(hasBanquetEnded);
                if (hasBanquetEnded)
                {
                    // DialogueTriggerにセリフを登録
                    var dialogueTrigger = otohimeObject.GetComponent<DialogueTrigger>();
                    if (dialogueTrigger != null)
                    {
                        var settings = CommonGameSettings.Settings ?? Resources.Load<GameSettingsData>("GameSettings");
                        var nodes = new System.Collections.Generic.List<DialogueNode>
                        {
                            new DialogueNode
                            {
                                speakerName = "おとひめ",
                                text = "あら、どうしましょう。あの子、あんな危険な場所まで逃げていっちゃうなんて……。",
                                speakerSprite = settings != null ? settings.otohimeFaceSadness : null
                            },
                            new DialogueNode
                            {
                                speakerName = "うらしまたろう",
                                text = "おとひめ様、何かあっただか？",
                                speakerSprite = settings != null ? settings.taroFaceNormal : null
                            },
                            new DialogueNode
                            {
                                speakerName = "おとひめ",
                                text = "私の大切なペットの熱帯魚が、城の外層部まで逃げてしまったの。あそこはとても危険な場所なのよ。どうにかして連れ戻したいのだけれど……。",
                                speakerSprite = settings != null ? settings.otohimeFaceSadness : null,
                                hasChoices = true,
                                choice1Text = "おらが捕まえてくるだ！",
                                choice2Text = "危ないのは苦手だっぺ…",
                                choice1NextNodes = new System.Collections.Generic.List<DialogueNode>
                                {
                                    new DialogueNode
                                    {
                                        speakerName = "うらしまたろう",
                                        text = "おらが捕まえてくるだ！おとひめ様の大切な魚なら、ほっておけねぇ。",
                                        speakerSprite = settings != null ? settings.taroFaceNormal : null
                                    },
                                    new DialogueNode
                                    {
                                        speakerName = "おとひめ",
                                        text = "ありがとう、たろう！本当に頼もしいわ！そのエリアへ行くなら、倉庫にある「イカ墨ジェット」を使いなさい。それがあれば、空中で直線的に突き進むことができるはずよ。",
                                        speakerSprite = settings != null ? settings.otohimeFaceJoy : null
                                    },
                                    new DialogueNode
                                    {
                                        speakerName = "おとひめ",
                                        text = "そのエリアは、宴会場の下にあるわ。そこにある「イカ墨ジェット」を使いなさい。それがあれば、空中で直線的に突き進むことができるはずよ。",
                                        speakerSprite = settings != null ? settings.otohimeFaceCute : null
                                    },
                                    new DialogueNode
                                    {
                                        speakerName = "うらしまたろう",
                                        text = "お魚さん、待っててけろよ！",
                                        speakerSprite = settings != null ? settings.taroFaceNormal : null
                                    }
                                },
                                choice2NextNodes = new System.Collections.Generic.List<DialogueNode>
                                {
                                    new DialogueNode
                                    {
                                        speakerName = "おとひめ",
                                        text = "……そうよね、無理はしないで。もし気が向いたら、お願いね。",
                                        speakerSprite = settings != null ? settings.otohimeFaceSadness : null
                                    }
                                }
                            }
                        };
                        dialogueTrigger.SetDialogueNodes(nodes);
                    }
                }
            }
        }
    }
}
