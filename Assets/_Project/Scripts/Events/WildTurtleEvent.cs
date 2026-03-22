using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueTrigger))]

public class WildTurtleEvent : MonoBehaviour
{
    private DialogueTrigger dialogueTrigger;
    [Tooltip("このイベントが紐づくGameScreen。未設定なら自身から取得")]
    [SerializeField] private GameScreen gameScreen;

    private void Awake()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();
        if (gameScreen == null)
        {
            gameScreen = GetComponent<GameScreen>();
        }
        // GameScreenのonScreenLoadedEventに登録
        if (gameScreen != null)
        {
            gameScreen.onScreenLoadedEvent += OnScreenLoaded;
        }
        else
        {
            // GameScreenが無い場合はAwake時に即設定
            SetupMissionDialogue();
        }
    }

    private void OnDestroy()
    {
        if (gameScreen != null)
        {
            gameScreen.onScreenLoadedEvent -= OnScreenLoaded;
        }
    }

    private void OnScreenLoaded()
    {
        SetupMissionDialogue();
    }

    private void SetupMissionDialogue()
    {
        var settings = CommonGameSettings.Settings ?? Resources.Load<GameSettingsData>("GameSettings");
        if (settings == null)
        {
            Debug.LogWarning("WildTurtleEvent: GameSettingsが見つかりません。");
            return;
        }

        // ミッション開始ダイアログ
        var missionStartNodes = new List<DialogueNode>
        {
            new DialogueNode
            {
                speakerName = "おとひめ",
                text = "困ったわ……。外でお魚さんたちと遊べないじゃない...。",
                speakerSprite = settings.otohimeFaceSadness
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "おとひめ様、どうしたんだべ？そんなに難しい顔して。",
                speakerSprite = settings.taroFaceNormal
            },
            new DialogueNode
            {
                speakerName = "おとひめ",
                text = "ああ、太郎さん。実は、城の近くに住む巨大なウミガメが、急に暴れだしてしまったの。原因はわからないのだけれど、このままでは被害が出てしまうわ。誰か、あのカメを鎮めてくれる勇気ある者はいないかしら……。",
                speakerSprite = settings.otohimeFaceSadness,
                hasChoices = true,
                choice1Text = "おらに任せてけろ！",
                choice2Text = "それは怖そうだっぺ……",
                choice1NextNodes = new List<DialogueNode>
                {
                    new DialogueNode
                    {
                        speakerName = "おとひめ",
                        text = "まあ、受けてくれるのね！ありがとう、太郎さん。",
                        speakerSprite = settings.otohimeFaceJoy,
                        OnNodeStart = () => {
                            // ミッション受諾フラグを立てる
                            FlagManager.Instance?.SetFlag(FlagManager.FlagKey.WildTurtleMissionStarted.ToString(), true);
                            // 受諾後の台詞に切り替え
                            if (dialogueTrigger != null)
                            {
                                // missionAcceptedNodesを再セット
                                var acceptedNodes = new List<DialogueNode>
                                {
                                    new DialogueNode
                                    {
                                        speakerName = "おとひめ",
                                        text = "あのカメはこの先にいるわ。でも気をつけて。すごく大きくて凶暴だから。",
                                        speakerSprite = settings.otohimeFaceSerious
                                    }
                                };
                                dialogueTrigger.SetDialogueNodes(acceptedNodes);
                            }
                        }
                    },
                    new DialogueNode
                    {
                        speakerName = "おとひめ",
                        text = "あのカメはこの先にいるわ。でも気をつけて。すごく大きくて凶暴だから。",
                        speakerSprite = settings.otohimeFaceSerious
                    },
                    new DialogueNode
                    {
                        speakerName = "うらしまたろう",
                        text = "大丈夫だ！おらがガツンと言い聞かせてくるだ！",
                        speakerSprite = settings.taroFaceNormal
                    }
                },
                choice2NextNodes = new List<DialogueNode>
                {
                    new DialogueNode
                    {
                        speakerName = "おとひめ",
                        text = "……そうよね、無理はしないで。もし気が向いたら、お願いね。",
                        speakerSprite = settings.otohimeFaceSadness
                    }
                }
            }
        };

        // ミッション受諾後、報酬未受取時の台詞
        var missionAcceptedNodes = new List<DialogueNode>
        {
            new DialogueNode
            {
                speakerName = "おとひめ",
                text = "あのカメはこの先にいるわ。でも気をつけて。すごく大きくて凶暴だから。",
                speakerSprite = settings.otohimeFaceSerious
            }
        };

        // ミッションクリアダイアログ
        var missionClearNodes = new List<DialogueNode>
        {
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "おとひめ様、暴れてたカメさん、なんとか大人しくさせてきただ。",
                speakerSprite = settings.taroFaceNormal
            },
            new DialogueNode
            {
                speakerName = "おとひめ",
                text = "素晴らしいわ、太郎さん！これでまた外でおさかなさんたちとあそべるわ。",
                speakerSprite = settings.otohimeFaceJoy
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "んだ。なんだかカメさん、苦しそうだったべ。今はぐっすり眠ってるだ。",
                speakerSprite = settings.taroFaceNormal
            },
            new DialogueNode
            {
                speakerName = "おとひめ",
                text = "勇敢なあなたに、竜宮城に伝わる秘宝「大海の籠手」を授けるわ。これがあれば、どんなに硬い岩でも砕くことができるはずよ。",
                speakerSprite = settings.otohimeFaceJoy
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "うおぉ！なんだべ、これ！体の中から力がモリモリ湧いてくるっぺ！",
                speakerSprite = settings.taroFaceSurprise,
                OnNodeStart = () =>
                {
                    FlagManager.Instance?.SetFlag(FlagManager.FlagKey.WildTurtleMissionFinished.ToString(), true);
                    // アビリティ解放
                    if (PlayerAbilityManager.Instance != null)
                    {
                        PlayerAbilityManager.Instance.UnlockAbility("IncreaseAttack");
                    }
                    else
                    {
                        Debug.LogWarning("WildTurtleEvent: PlayerAbilityManagerのインスタンスが見つかりません。");
                    }
                    // アイテム取得UI表示（任意: UIManager経由で取得）
                    var itemGetCanvas = UIManager.Instance?.GetView<ItemGetCanvas>();
                    if (itemGetCanvas != null)
                    {
                        itemGetCanvas.ShowItem(settings.increaseAttackSprite, "大海の籠手", "装備すると攻撃力が大幅に上昇する。特定のオブジェクトを破壊できるようになる。");
                    }
                }
            }
        };

        if (dialogueTrigger != null)
        {
            // 進行度フラグを参照して会話を切り替え
            bool missionFinished = false;
            bool missionRewardAvailable = false;
            bool missionStarted = false;
            if (FlagManager.Instance != null)
            {
                missionFinished = FlagManager.Instance.GetFlag(FlagManager.FlagKey.WildTurtleMissionFinished.ToString(), false);
                missionRewardAvailable = FlagManager.Instance.GetFlag(FlagManager.FlagKey.WildTurtleMissionRewardAvailable.ToString(), false);
                // 受諾フラグ（未定義ならfalse）
                missionStarted = FlagManager.Instance.GetFlag(FlagManager.FlagKey.WildTurtleMissionStarted.ToString(), false);
            }

            if (missionFinished)
            {
                // 既にクリア済みなら何も表示しない or お礼だけ
                dialogueTrigger.SetDialogueNodes(new List<DialogueNode> {
                    new DialogueNode {
                        speakerName = "おとひめ",
                        text = "本当にありがとう、太郎さん。おかげでみんな安心して外で遊べるわ。",
                        speakerSprite = settings.otohimeFaceJoy
                    }
                });
            }
            else if (missionRewardAvailable)
            {
                // 報酬受け取り会話
                dialogueTrigger.SetDialogueNodes(missionClearNodes);
            }
            else if (missionStarted)
            {
                // ミッション受諾後、報酬未受取時
                dialogueTrigger.SetDialogueNodes(missionAcceptedNodes);
                dialogueTrigger.onDialogueEnd = null;
            }
            else
            {
                // ミッション開始会話
                dialogueTrigger.SetDialogueNodes(missionStartNodes);
                dialogueTrigger.onDialogueEnd = null;
            }
        }
        else
        {
            Debug.LogWarning("WildTurtleEvent: DialogueTriggerコンポーネントが見つかりません。");
        }
    }
}
