using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueTrigger))]
public class FoodWarehouseMissionEvent : MonoBehaviour
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
            Debug.LogWarning("FoodWarehouseMissionEvent: GameSettingsが見つかりません。");
            return;
        }

        // ミッション開始ダイアログ
        List<DialogueNode> missionStartNodes;
        bool hasIncreaseAttack = false;
        if (FlagManager.Instance != null)
        {
            hasIncreaseAttack = FlagManager.Instance.GetFlag(FlagManager.FlagKey.HasIncreaseAttack.ToString(), false);
        }

        if (hasIncreaseAttack)
        {
            missionStartNodes = new List<DialogueNode>
            {
                new DialogueNode
                {
                    speakerName = "現代人E",
                    text = "ああ、神様……。すぐそこに、子供たちの命を繋ぐ食糧があるのに……。",
                    speakerSprite = settings.modernPeopleEFaceSadness
                },
                new DialogueNode
                {
                    speakerName = "うらしまたろう",
                    text = "おばちゃん、どうしたんだべ？そんなに悲しい顔して 。",
                    speakerSprite = settings.taroFaceConfusion
                },
                new DialogueNode
                {
                    speakerName = "現代人E",
                    text = "……この先には食糧庫があるの 。でも、巨大な瓦礫が入り口を完全にふさいでしまって……。私たち大人がいくら束になっても、びくともしないのよ 。",
                    speakerSprite = settings.modernPeopleEFaceSadness,
                    hasChoices = true,
                    choice1Text = "おらが壊してみせるだ！",
                    choice2Text = "無理だっぺ…",
                    choice1NextNodes = new List<DialogueNode>
                    {
                        new DialogueNode
                        {
                            speakerName = "うらしまたろう",
                            text = "泣かねぇでけろ。おらのこの「大海の籠手」なら、この岩も壊せる気がするだ 。竜宮城でカメさんを助けた時みてぇに、おらの力、誰かのために使わせてけろ ！",
                            speakerSprite = settings.taroFaceConfidence,
                            OnNodeStart = () => {
                                // ミッション受諾フラグを立てる
                                FlagManager.Instance?.SetFlag(FlagManager.FlagKey.FoodWarehouseMissionStarted.ToString(), true);
                                // 受諾後の台詞に切り替え
                                if (dialogueTrigger != null)
                                {
                                    var acceptedNodes = new List<DialogueNode>
                                    {
                                        new DialogueNode
                                        {
                                            speakerName = "現代人E",
                                            text = "な、何を言っているの？その細い腕でこの岩山に挑むなんて、自分を傷つけるだけよ！子供たちのために気持ちは嬉しいけれど、無茶だけはやめて。",
                                            speakerSprite = settings.modernPeopleEFaceWorry
                                        },
                                        new DialogueNode
                                        {
                                            speakerName = "うらしまたろう",
                                            text = "心配してくれてありがとよ。でも、心配ないだ。",
                                            speakerSprite = settings.taroFaceJoy
                                        }
                                    };
                                    dialogueTrigger.SetDialogueNodes(acceptedNodes);
                                }
                            }
                        },
                        new DialogueNode
                        {
                            speakerName = "現代人E",
                            text = "な、何を言っているの？その細い腕でこの岩山に挑むなんて、自分を傷つけるだけよ！子供たちのために気持ちは嬉しいけれど、無茶だけはやめて。",
                            speakerSprite = settings.modernPeopleEFaceWorry
                        },
                        new DialogueNode
                        {
                            speakerName = "うらしまたろう",
                            text = "心配してくれてありがとよ。でも、心配ないだ。",
                            speakerSprite = settings.taroFaceJoy
                        }
                    },
                    choice2NextNodes = new List<DialogueNode>
                    {
                        new DialogueNode
                        {
                            speakerName = "現代人E",
                            text = "……そうよね、無理はしないで。もし気が向いたら、お願いね。",
                            speakerSprite = settings.modernPeopleEFaceSadness
                        }
                    }
                }
            };
        }
        else
        {
            missionStartNodes = new List<DialogueNode>
            {
                new DialogueNode
                {
                    speakerName = "現代人E",
                    text = "ああ、神様……。すぐそこに、子供たちの命を繋ぐ食糧があるのに……。",
                    speakerSprite = settings.modernPeopleEFaceSadness
                },
                new DialogueNode
                {
                    speakerName = "うらしまたろう",
                    text = "おばちゃん、どうしたんだべ？そんなに悲しい顔して 。",
                    speakerSprite = settings.taroFaceSadness
                },
                new DialogueNode
                {
                    speakerName = "現代人E",
                    text = "……この先には食糧庫があるの 。でも、巨大な瓦礫が入り口を完全にふさいでしまって……。私たち大人がいくら束になっても、びくともしないのよ 。",
                    speakerSprite = settings.modernPeopleEFaceSadness
                },
                new DialogueNode
                {
                    speakerName = "うらしまたろう",
                    text = "それは大変だべな。何かいい方法はないだろか…。",
                    speakerSprite = settings.taroFaceConfusion
                }
            };
        }

        // ミッション受諾後、報酬未受取時の台詞
        var missionAcceptedNodes = new List<DialogueNode>
        {
            new DialogueNode
            {
                speakerName = "現代人E",
                text = "な、何を言っているの？その細い腕でこの岩山に挑むなんて、自分を傷つけるだけよ！子供たちのために気持ちは嬉しいけれど、無茶だけはやめて。",
                speakerSprite = settings.modernPeopleEFaceWorry

            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "心配してくれてありがとよ。でも、心配ないだ。",
                speakerSprite = settings.taroFaceJoy

            }
        };

        // ミッションクリアダイアログ
        var missionClearNodes = new List<DialogueNode>
        {
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "おばちゃん、やっただ！岩を全部壊して、倉庫まで行けるようになったっぺ！",
                speakerSprite = settings.taroFaceJoy

            },
            new DialogueNode
            {
                speakerName = "現代人E",
                text = "……！嘘……あの巨大な瓦礫の山が、跡形もなく砕かれているなんて…… 。あなた、一体何者なの？",
                speakerSprite = settings.modernPeopleEFaceSurprise
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "へへっ、竜宮城でもらった籠手の力はすごかっただ ！ほら、これで中にある食べ物を、子供たちにいっぱい食べさせてやれるべ。",
                speakerSprite = settings.taroFaceJoy
            },
            new DialogueNode
            {
                speakerName = "現代人E",
                text = "……信じられない。あんなに絶望していたのが馬鹿みたいね。本当に感謝しているわ。これで、あの子たちにひもじい思いをさせずに済むのね 。",
                speakerSprite = settings.modernPeopleEFaceJoy
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "おばちゃんの涙が止まって、本当によかっただ。",
                speakerSprite = settings.taroFaceJoy
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
                missionFinished = FlagManager.Instance.GetFlag(FlagManager.FlagKey.FoodWarehouseMissionFinished.ToString(), false);
                missionRewardAvailable = FlagManager.Instance.GetFlag(FlagManager.FlagKey.FoodWarehouseMissionRewardAvailable.ToString(), false);
                missionStarted = FlagManager.Instance.GetFlag(FlagManager.FlagKey.FoodWarehouseMissionStarted.ToString(), false);
            }

            if (missionFinished)
            {
                dialogueTrigger.SetDialogueNodes(new List<DialogueNode> {
                    new DialogueNode {
                        speakerName = "現代人E",
                        text = "本当にありがとう。おかげで子供たちも助かったわ。",
                        speakerSprite = settings.modernPeopleEFaceJoy
                    }
                });
            }
            else if (missionRewardAvailable)
            {
                dialogueTrigger.SetDialogueNodes(missionClearNodes);
                dialogueTrigger.onDialogueEnd = () =>
                {
                    FlagManager.Instance?.SetFlag(FlagManager.FlagKey.FoodWarehouseMissionFinished.ToString(), true);
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
            Debug.LogWarning("FoodWarehouseMissionEvent: DialogueTriggerコンポーネントが見つかりません。");
        }
    }
}
