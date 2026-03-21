using UnityEngine;
using UnityEngine.Events;

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
    [Tooltip("地上に戻るイベントオブジェクト")]
    [SerializeField] private GameObject returnToTheGroundEvent;
    [SerializeField] private GameObject playerBlock;

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
            bool petMissionRewardAvailable = FlagManager.Instance.GetFlag(FlagManager.FlagKey.PetMissionRewardAvailable.ToString(), false);
            bool petMissionFinished = FlagManager.Instance.GetFlag(FlagManager.FlagKey.PetMissionFinished.ToString(), false);
            bool hasVisitedFuture = FlagManager.Instance.GetFlag(FlagManager.FlagKey.HasVisitedFuture.ToString(), false);

            if (banquetEvent != null)
            {
                // 宴が終わっていない(false)場合はアクティブ、終わっている(true)場合は非アクティブ
                banquetEvent.SetActive(!hasBanquetEnded);
            }
            if (otohimeObject != null)
            {
                // ペットミッション達成時は乙姫を一時的にアクティブ、その後非アクティブに
                otohimeObject.SetActive(hasBanquetEnded && !petMissionFinished);
                var dialogueTrigger = otohimeObject.GetComponent<DialogueTrigger>();
                if (dialogueTrigger != null)
                {
                    var settings = CommonGameSettings.Settings ?? Resources.Load<GameSettingsData>("GameSettings");
                    System.Collections.Generic.List<DialogueNode> nodes;
                    if (petMissionRewardAvailable && !petMissionFinished)
                    {
                        // ミッション達成時の会話
                        nodes = new System.Collections.Generic.List<DialogueNode>
                        {
                            new DialogueNode
                            {
                                speakerName = "うらしまたろう",
                                text = "おとひめ様、お魚さん捕まえてきただ！元気にしてるっぺ。",
                                speakerSprite = settings != null ? settings.taroFaceNormal : null
                            },
                            new DialogueNode
                            {
                                speakerName = "おとひめ",
                                text = "ああ、よかった！戻ってきてくれたのね。本当にありがとう、太郎さん。",
                                speakerSprite = settings != null ? settings.otohimeFaceJoy : null
                            },
                            new DialogueNode
                            {
                                speakerName = "熱帯魚",
                                text = "プクプク……（助けてくれてありがとう！）",
                                speakerSprite = settings != null ? settings.petIconJoy : null
                            },
                            new DialogueNode
                            {
                                speakerName = "おとひめ",
                                text = "ふふ、この子も感謝しているわ。よしよし、もう勝手に出ていっちゃダメよ。",
                                speakerSprite = settings != null ? settings.otohimeFaceJoy : null
                            },
                            new DialogueNode
                            {
                                speakerName = "うらしまたろう",
                                text = "お魚さんが無事で、おらも嬉しいだ。イカ墨の勢い、凄くてびっくりしたっぺ！",
                                speakerSprite = settings != null ? settings.taroFaceSurprise : null
                            }
                        };
                        // 会話終了時に乙姫を非アクティブ化し、PetMissionFinishedフラグを立てる
                        dialogueTrigger.onDialogueEnd = () =>
                        {
                            otohimeObject.SetActive(false);
                            FlagManager.Instance?.SetFlag(FlagManager.FlagKey.PetMissionFinished.ToString(), true);
                        };
                    }
                    else if (hasBanquetEnded)
                    {
                        // 通常のミッション開始前会話
                        nodes = new System.Collections.Generic.List<DialogueNode>
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
                                        speakerSprite = settings != null ? settings.taroFaceNormal : null,
                                        OnNodeStart = () => {
                                            var nodes = new System.Collections.Generic.List<DialogueNode>
                                           {
                                                new DialogueNode
                                                {
                                                    speakerName = "おとひめ",
                                                    text = "そのエリアは、宴会場の下にあるわ。そこにある「イカ墨ジェット」を使いなさい。それがあれば、空中で直線的に突き進むことができるはずよ。",
                                                    speakerSprite = settings != null ? settings.otohimeFaceJoy : null
                                                }
                                            };
                                            dialogueTrigger.SetDialogueNodes(nodes);
                                        }
                                    },
                                    new DialogueNode
                                    {
                                        speakerName = "おとひめ",
                                        text = "ありがとう、たろう！本当に頼もしいわ！",
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
                        dialogueTrigger.onDialogueEnd = null;
                    }
                    else
                    {
                        nodes = null;
                        dialogueTrigger.onDialogueEnd = null;
                    }
                    if (nodes != null)
                    {
                        dialogueTrigger.SetDialogueNodes(nodes);
                    }
                }
            }

            // returnToTheGroundEventのアクティブ制御
            if (returnToTheGroundEvent != null)
            {
                returnToTheGroundEvent.SetActive(hasVisitedFuture);
            }
            if (playerBlock != null)
            {
                playerBlock.SetActive(hasVisitedFuture);
            }
        }
    }
}
