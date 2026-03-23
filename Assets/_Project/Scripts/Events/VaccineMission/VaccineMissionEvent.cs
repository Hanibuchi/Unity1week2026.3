using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueTrigger))]
public class VaccineMissionEvent : MonoBehaviour
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
            Debug.LogWarning("VaccineMissionEvent: GameSettingsが見つかりません。");
            return;
        }

        // missionFinished状態に応じてSpriteを切り替え
        bool missionFinished = false;
        if (FlagManager.Instance != null)
        {
            missionFinished = FlagManager.Instance.GetFlag(FlagManager.FlagKey.VaccineMissionFinished.ToString(), false);
        }
        if (targetSpriteRenderer != null)
        {
            targetSpriteRenderer.sprite = missionFinished ? spriteWhenFinished : spriteWhenNotFinished;
        }

        // ミッション開始ダイアログ
        List<DialogueNode> missionStartNodes;
        bool hasJetDash = false;
        if (FlagManager.Instance != null)
        {
            hasJetDash = FlagManager.Instance.GetFlag(FlagManager.FlagKey.HasJetDash.ToString(), false);
        }

        if (hasJetDash)
        {
            missionStartNodes = new List<DialogueNode>
            {
                new DialogueNode
                {
                    speakerName = "現代人D",
                    text = "……くっ、目の前に答えがあるのに、辿り着けないとは。",
                    speakerSprite = settings.modernPeopleDFaceWorry
                },
                new DialogueNode
                {
                    speakerName = "うらしまたろう",
                    text = "おじさん、どうしたんだべ？難しい顔して、何か探し物か？",
                    speakerSprite = settings.taroFaceConfusion
                },
                new DialogueNode
                {
                    speakerName = "現代人D",
                    text = "……ああ。あそこの医療機関には、まだワクチンが残っているはずなんだ。伝染病の蔓延を止めるには、どうしてもそれが必要なのだが……。",
                    speakerSprite = settings.modernPeopleDFaceWorry
                },
                new DialogueNode
                {
                    speakerName = "うらしまたろう",
                    text = "だったら、さっさと取りに行けばいいべ？",
                    speakerSprite = settings.taroFaceConfusion
                },
                new DialogueNode
                {
                    speakerName = "現代人D",
                    text = "そうもいかない。道中のガラスが粉々に割れていて、まるで刃物の山のように立ちはだかっている。生身で足を踏み入れれば、ワクチンを手にする前にこちらが動けなくなるだろう。",
                    speakerSprite = settings.modernPeopleDFaceWorry,
                    hasChoices = true,
                    choice1Text = "おらが取ってきてやるだ！",
                    choice2Text = "おらも無理だっぺ…",
                    choice1NextNodes = new List<DialogueNode>
                    {
                        new DialogueNode
                        {
                            speakerName = "うらしまたろう",
                            text = "おら、竜宮城でもらったこの「イカ墨ジェット」があるから大丈夫だ。ガラスの上を飛び越えて、必ず取ってきてやるだ！",
                            speakerSprite = settings.taroFaceConfidence,
                            OnNodeStart = () => {
                                FlagManager.Instance?.SetFlag(FlagManager.FlagKey.VaccineMissionStarted.ToString(), true);
                                if (dialogueTrigger != null)
                                {
                                    var acceptedNodes = new List<DialogueNode>
                                    {
                                        new DialogueNode
                                        {
                                            speakerName = "現代人D",
                                            text = "イカ墨……？子供の遊びに付き合ってはいられない。早くここを通る方法を考えなければ。",
                                            speakerSprite = settings.modernPeopleDFaceWorry
                                        }
                                    };
                                    dialogueTrigger.SetDialogueNodes(acceptedNodes);
                                }
                            }
                        },
                        new DialogueNode
                        {
                            speakerName = "現代人D",
                            text = "イカ墨……？子供の遊びに付き合ってはいられない。早くここを通る方法を考えなければ。",
                            speakerSprite = settings.modernPeopleDFaceWorry
                        }
                    },
                    choice2NextNodes = new List<DialogueNode>
                    {
                        new DialogueNode
                        {
                            speakerName = "現代人D",
                            text = "……そうだ、無理はするな。",
                            speakerSprite = settings.modernPeopleDFaceWorry
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
                    speakerName = "現代人D",
                    text = "……くっ、目の前に答えがあるのに、辿り着けないとは。",
                    speakerSprite = settings.modernPeopleDFaceWorry
                },
                new DialogueNode
                {
                    speakerName = "うらしまたろう",
                    text = "おじさん、どうしたんだべ？難しい顔して、何か探し物か？",
                    speakerSprite = settings.taroFaceConfusion
                },
                new DialogueNode
                {
                    speakerName = "現代人D",
                    text = "……ああ。あそこの医療機関には、まだワクチンが残っているはずなんだ。伝染病の蔓延を止めるには、どうしてもそれが必要なのだが……。",
                    speakerSprite = settings.modernPeopleDFaceWorry
                },
                new DialogueNode
                {
                    speakerName = "うらしまたろう",
                    text = "だったら、さっさと取りに行けばいいべ？",
                    speakerSprite = settings.taroFaceConfusion
                },
                new DialogueNode
                {
                    speakerName = "現代人D",
                    text = "そうもいかない。道中のガラスが粉々に割れていて、まるで刃物の山のように立ちはだかっている。生身で足を踏み入れれば、ワクチンを手にする前にこちらが動けなくなるだろう。",
                    speakerSprite = settings.modernPeopleDFaceWorry
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
                speakerName = "現代人D",
                text = "イカ墨……？子供の遊びに付き合ってはいられない。早くここを通る方法を考えなければ。",
                speakerSprite = settings.modernPeopleDFaceWorry
            }
        };

        // ミッションクリアダイアログ
        var missionClearNodes = new List<DialogueNode>
        {
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "おじさん、取ってきただ！これがその「ワクチム」ってやつだべ？",
                speakerSprite = settings.taroFaceJoy
            },
            new DialogueNode
            {
                speakerName = "現代人D",
                text = "……！本当に、あのガラスの海を越えて戻ってきたというのか。信じがたいが、確かにこれは私が探していた「ワクチン」だ。",
                speakerSprite = settings.modernPeopleDFaceSurprise
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "へへっ、空を飛んでるみたいで気持ちよかっただ！これでお病気の人たちを助けられるべ？",
                speakerSprite = settings.taroFaceJoy
            },
            new DialogueNode
            {
                speakerName = "現代人D",
                text = "ああ、助かるとも。君のおかげで、救える命がいくつもある。……正直に言えば、君のような少年を危険に晒した自分を恥じているが、今はただ、感謝させてくれ。",
                speakerSprite = settings.modernPeopleDFaceWorry
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "おじさんも、誰かを助けようと一生懸命だったんだべ。おらの力が役に立って、本当によかっただ！",
                speakerSprite = settings.taroFaceJoy
            }
        };

        if (dialogueTrigger != null)
        {
            bool missionRewardAvailable = false;
            bool missionStarted = false;
            if (FlagManager.Instance != null)
            {
                missionRewardAvailable = FlagManager.Instance.GetFlag(FlagManager.FlagKey.VaccineMissionRewardAvailable.ToString(), false);
                missionStarted = FlagManager.Instance.GetFlag(FlagManager.FlagKey.VaccineMissionStarted.ToString(), false);
            }

            if (missionFinished)
            {
                dialogueTrigger.SetDialogueNodes(new List<DialogueNode> {
                    new DialogueNode {
                        speakerName = "現代人D",
                        text = "君のおかげで、救える命がいくつもある。……本当にありがとう。",
                        speakerSprite = settings.modernPeopleDFaceJoy
                    }
                });
            }
            else if (missionRewardAvailable)
            {
                dialogueTrigger.SetDialogueNodes(missionClearNodes);
                dialogueTrigger.onDialogueEnd = () =>
                {
                    FlagManager.Instance?.SetFlag(FlagManager.FlagKey.VaccineMissionFinished.ToString(), true);
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
            Debug.LogWarning("VaccineMissionEvent: DialogueTriggerコンポーネントが見つかりません。");
        }
    }
}
