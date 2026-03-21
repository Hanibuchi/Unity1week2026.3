using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueTrigger))]
public class PlantMissionEvent : MonoBehaviour
{
    private DialogueTrigger dialogueTrigger;
    [Tooltip("このイベントが紐づくGameScreen。未設定なら自身から取得")]
    [SerializeField] private GameScreen gameScreen;

    [Header("状態によって切り替えるSpriteRenderer")]
    [SerializeField] private SpriteRenderer targetSpriteRenderer;
    [SerializeField] private Sprite spriteWhenFinished;
    [SerializeField] private Sprite spriteWhenNotFinished;
    private void Awake()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();
        if (gameScreen == null)
        {
            gameScreen = GetComponent<GameScreen>();
        }
        if (gameScreen != null)
        {
            gameScreen.onScreenLoadedEvent += OnScreenLoaded;
        }
        else
        {
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
            Debug.LogWarning("PlantMissionEvent: GameSettingsが見つかりません。");
            return;
        }

        // missionFinished状態に応じてSpriteを切り替え
        bool missionFinished = false;
        if (FlagManager.Instance != null)
        {
            missionFinished = FlagManager.Instance.GetFlag(FlagManager.FlagKey.ChemicalPlantMissionFinished.ToString(), false);
        }
        if (targetSpriteRenderer != null)
        {
            targetSpriteRenderer.sprite = missionFinished ? spriteWhenFinished : spriteWhenNotFinished;
        }

        // ミッション開始ダイアログ
        List<DialogueNode> missionStartNodes;
        bool hasDownAttack = false;
        if (FlagManager.Instance != null)
        {
            hasDownAttack = FlagManager.Instance.GetFlag(FlagManager.FlagKey.HasAttackDown.ToString(), false);
        }

        if (hasDownAttack)
        {
            missionStartNodes = new List<DialogueNode>
            {
                new DialogueNode
                {
                    speakerName = "現代人B",
                    text = "おい、止まれ！この先へ行く気か？死にてぇのか！",
                    speakerSprite = settings.modernPeopleBFaceWorry
                },
                new DialogueNode
                {
                    speakerName = "うらしまたろう",
                    text = "なんだべ、急に。おら、この先に行ってみてぇんだ。",
                    speakerSprite = settings.taroFaceConfusion
                },
                new DialogueNode
                {
                    speakerName = "現代人B",
                    text = "バカを言うな。この先は変異した虫どものたまり場だ。俺だって近づくことすらできねぇ。囲まれたら最後、骨も残らねえぞ。……くっ、ここさえ通れればあいつらを全滅させられるんだが……。",
                    speakerSprite = settings.modernPeopleBFaceFlustered,
                },
                new DialogueNode
                {
                    speakerName = "うらしまたろう",
                    text = "…？あそこを抜ければ何かあるんだべ？",
                    speakerSprite = settings.taroFaceConfusion
                },
                new DialogueNode
                {
                    speakerName = "現代人B",
                    text = "この先にある化学プラントまで行ければ、強力な殺虫剤が手に入るはずなんだ。それさえあれば、この辺りの虫どもを一掃して、まともな生活を取り戻せるんだが……道があまりに危険すぎて、誰も辿り着けやしねぇ。",
                    speakerSprite = settings.modernPeopleBFaceFlustered,
                    hasChoices = true,
                    choice1Text = "おらが取ってくるだ！",
                    choice2Text = "そんなに怖ぇところなんだべか…",
                    choice1NextNodes = new List<DialogueNode>
                    {
                        new DialogueNode
                        {
                            speakerName = "うらしまたろう",
                            text = "……。みんな困ってるんだな。よし、おらが行って、そのサッチュウザイってやつを取ってくるだ！",
                            speakerSprite = settings.taroFaceConfidence,
                            OnNodeStart = () => {
                                FlagManager.Instance?.SetFlag(FlagManager.FlagKey.ChemicalPlantMissionStarted.ToString(), true);
                                if (dialogueTrigger != null)
                                {
                                    var acceptedNodes = new List<DialogueNode>
                                    {
                                        new DialogueNode
                                        {
                                            speakerName = "現代人B",
                                            text = "はあ！？正気かよ。お前みたいなガキが一人で行ってどうにかなる相手じゃねえんだ。いいから大人しく引き返せ。死にに行くようなもんだぞ！",
                                            speakerSprite = settings.modernPeopleBFaceWorry
                                        },
                                        new DialogueNode
                                        {
                                            speakerName = "うらしまたろう",
                                            text = "心配してくれてありがとよ。でも、おらには竜宮城で教わった「豊漁の急降下」があるだ！これなら、虫どもが溜まってるところも飛び越えていけるっぺ。おら、やってみるだ！",
                                            speakerSprite = settings.taroFaceJoy
                                        },
                                        new DialogueNode
                                        {
                                            speakerName = "現代人B",
                                            text = "リョウグウジョウ……？豊漁……？何を言ってやがる。おい、待て！……ったく、なんて無鉄砵な野郎だ。",
                                            speakerSprite = settings.modernPeopleBFaceFlustered
                                        }
                                    };
                                    dialogueTrigger.SetDialogueNodes(acceptedNodes);
                                }
                            }
                        },
                        new DialogueNode
                        {
                            speakerName = "現代人B",
                            text = "はあ！？正気かよ。お前みたいなガキが一人で行ってどうにかなる相手じゃねえんだ。いいから大人しく引き返せ。死にに行くようなもんだぞ！",
                            speakerSprite = settings.modernPeopleBFaceWorry
                        },
                        new DialogueNode
                        {
                            speakerName = "うらしまたろう",
                            text = "心配してくれてありがとよ。でも、おらには竜宮城で教わった「豊漁の急降下」があるだ！これなら、虫どもが溜まってるところも飛び越えていけるっぺ。おら、やってみるだ！",
                            speakerSprite = settings.taroFaceJoy
                        },
                        new DialogueNode
                        {
                            speakerName = "現代人B",
                            text = "リョウグウジョウ……？豊漁……？何を言ってやがる。おい、待て！……ったく、なんて無鉄砵な野郎だ。",
                            speakerSprite = settings.modernPeopleBFaceWorry
                        }
                    },
                    choice2NextNodes = new List<DialogueNode>
                    {
                        new DialogueNode
                        {
                            speakerName = "現代人B",
                            text = "……そうだ、無理はするな。",
                            speakerSprite = settings.modernPeopleBFaceWorry
                        }
                    }
                }
            }
        ;
        }
        else
        {
            missionStartNodes = new List<DialogueNode>
            {
                new DialogueNode
                {
        speakerName = "現代人B",
                    text = "おい、止まれ！この先へ行く気か？死にてぇのか！",
                    speakerSprite = settings.modernPeopleBFaceWorry
    },
                new DialogueNode
                {
        speakerName = "うらしまたろう",
                    text = "なんだべ、急に。おら、この先に行ってみてぇんだ。",
                    speakerSprite = settings.taroFaceConfusion
    },
                new DialogueNode
                {
        speakerName = "現代人B",
                    text = "バカを言うな。この先は変異した虫どものたまり場だ。俺だって近づくことすらできねぇ。囲まれたら最後、骨も残らねえぞ。……くっ、ここさえ通れればあいつらを全滅させられるんだが……。",
                    speakerSprite = settings.modernPeopleBFaceWorry
    },
                new DialogueNode
                {
        speakerName = "うらしまたろう",
                    text = "…？あそこを抜ければ何かあるんだべ？",
                    speakerSprite = settings.taroFaceConfusion
    },
                new DialogueNode
                {
        speakerName = "現代人B",
                    text = "ああ。あの先にある化学プラントまで行ければ、強力な殺虫剤が手に入るはずなんだ。それさえあれば、この辺りの虫どもを一掃して、まともな生活を取り戻せるんだが……道があまりに危険すぎて、誰も辿り着けやしねぇ。",
                    speakerSprite = settings.modernPeopleBFaceWorry
    },
                new DialogueNode
                {
        speakerName = "うらしまたろう",
                    text = "それは大変だべな…。",
                    speakerSprite = settings.taroFaceSadness
    }
};
        }

        // ミッション受諾後、報酬未受取時の台詞
        var missionAcceptedNodes = new List<DialogueNode>
        {
            new DialogueNode
            {
                speakerName = "現代人B",
                text = "はあ！？正気かよ。お前みたいなガキが一人で行ってどうにかなる相手じゃねえんだ。いいから大人しく引き返せ。死にに行くようなもんだぞ！",
                speakerSprite = settings.modernPeopleBFaceWorry
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "心配してくれてありがとよ。でも、おらには竜宮城で教わった「豊漁の急降下」があるだ！これなら、虫どもが溜まってるところも飛び越えていけるっぺ。おら、やってみるだ！",
                speakerSprite = settings.taroFaceJoy
            },
            new DialogueNode
            {
                speakerName = "現代人B",
                text = "リョウグウジョウ……？豊漁……？何を言ってやがる。おい、待て！……ったく、なんて無鉄砵な野郎だ。",
                speakerSprite = settings.modernPeopleBFaceWorry
            }
        };

        // ミッションクリアダイアログ
        var missionClearNodes = new List<DialogueNode>
        {
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "ハチさん、持ってきただ！これがお目当てのサッチュウザイだべ？",
                speakerSprite = settings.taroFaceJoy
            },
            new DialogueNode
            {
                speakerName = "現代人B",
                text = "……！？ウソだろ、本当に行って戻ってきやがったのか。お前、あの虫の群れをどうやって……。",
                speakerSprite = settings.modernPeopleBFaceSurprise
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "へへっ、言ったべ？おらの技があれば大丈夫だって。ほら、受け取ってけろ！",
                speakerSprite = settings.taroFaceJoy
            },
            new DialogueNode
            {
                speakerName = "現代人B",
                text = "……ありがてぇ。いや、その前に、あれほど危ねえって言ったのによ！もしお前に何かあったらどうするつもりだったんだ！……だが、助かった。これで安全にプラントまで行ける道を切り拓ける。",
                speakerSprite = settings.modernPeopleBFaceWorry
            },
            new DialogueNode
            {
                speakerName = "現代人B",
                text = "お前のおかげで、この場所にも少しは希望が見えてきたぜ。",
                speakerSprite = settings.modernPeopleBFaceJoy
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "よかった……。ハチさんは怒ってるみたいだけど、本当は優しい人なんだべな。",
                speakerSprite = settings.taroFaceJoy
            }
        };

        if (dialogueTrigger != null)
        {
            bool missionRewardAvailable = false;
            bool missionStarted = false;
            if (FlagManager.Instance != null)
            {
                missionRewardAvailable = FlagManager.Instance.GetFlag(FlagManager.FlagKey.ChemicalPlantMissionRewardAvailable.ToString(), false);
                missionStarted = FlagManager.Instance.GetFlag(FlagManager.FlagKey.ChemicalPlantMissionStarted.ToString(), false);
            }

            if (missionFinished)
            {
                dialogueTrigger.SetDialogueNodes(new List<DialogueNode> {
                    new DialogueNode {
                        speakerName = "現代人B",
                        text = "お前のおかげで、この場所にも少しは希望が見えてきたぜ。",
                        speakerSprite = settings.modernPeopleBFaceJoy
                    }
                });
            }
            else if (missionRewardAvailable)
            {
                dialogueTrigger.SetDialogueNodes(missionClearNodes);
                dialogueTrigger.onDialogueEnd = () =>
                {
                    FlagManager.Instance?.SetFlag(FlagManager.FlagKey.ChemicalPlantMissionFinished.ToString(), true);
                    gameScreen?.OnScreenLoaded();
                };
            }
            else if (missionStarted)
            {
                dialogueTrigger.SetDialogueNodes(missionAcceptedNodes);
                dialogueTrigger.onDialogueEnd = null;
            }
            else
            {
                dialogueTrigger.SetDialogueNodes(missionStartNodes);
                dialogueTrigger.onDialogueEnd = null;
            }
        }
        else
        {
            Debug.LogWarning("PlantMissionEvent: DialogueTriggerコンポーネントが見つかりません。");
        }
    }
}
