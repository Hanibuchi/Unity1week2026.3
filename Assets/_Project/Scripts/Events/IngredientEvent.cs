using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueTrigger))]
public class IngredientEvent : MonoBehaviour
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
            Debug.LogWarning("IngredientEvent: GameSettingsが見つかりません。");
            return;
        }

        // ミッション開始ダイアログ
        var missionStartNodes = new List<DialogueNode>
        {
            new DialogueNode
            {
                speakerName = "おとひめ",
                text = "困ったわ……。このままでは、次の宴のメインディッシュが出せないわ。",
                speakerSprite = settings.otohimeFaceSadness
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "おとひめ様、どうしたんだべ？",
                speakerSprite = settings.taroFaceNormal
            },
            new DialogueNode
            {
                speakerName = "おとひめ",
                text = "実は、宴に欠かせない海鮮が足りなくなってしまったの。この先にいる魚たちからとれるんだけど、最近凶暴な魚が増えて大変なのよ。誰か、食材を集めてきてくれないかしら……。",
                speakerSprite = settings.otohimeFaceSadness,
                hasChoices = true,
                choice1Text = "おらが取ってくるだ！",
                choice2Text = "危ないのは嫌いだっぺ…",
                choice1NextNodes = new List<DialogueNode>
                {
                    new DialogueNode
                    {
                        speakerName = "うらしまたろう",
                        text = "おらが取ってくるだ！うまいもんのためなら、おら、どこへでも行くだ！",
                        speakerSprite = settings.taroFaceNormal,
                        OnNodeStart = () => {
                            FlagManager.Instance?.SetFlag(FlagManager.FlagKey.IngredientMissionStarted.ToString(), true);
                            if (dialogueTrigger != null)
                            {
                                var acceptedNodes = new List<DialogueNode>
                                {
                                    new DialogueNode
                                    {
                                        speakerName = "おとひめ",
                                        text = "ふふ、食いしん坊な太郎さんならそう言ってくれると思ったわ。期待しているわね。魚はこの先にいるわ。気をつけて行ってきてね。",
                                        speakerSprite = settings.otohimeFaceJoy
                                    }
                                };
                                dialogueTrigger.SetDialogueNodes(acceptedNodes);
                            }
                        }
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
                text = "ふふ、食いしん坊な太郎さんならそう言ってくれると思ったわ。期待しているわね。",
                speakerSprite = settings.otohimeFaceJoy
            }
        };

        // ミッションクリアダイアログ
        var missionClearNodes = new List<DialogueNode>
        {
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "おとひめ様、魚いっぱい集めてきただ！これで作れるだか？",
                speakerSprite = settings.taroFaceNormal
            },
            new DialogueNode
            {
                speakerName = "おとひめ",
                text = "完璧だわ！これがあれば、また最高に美味しい料理が作れるわね。",
                speakerSprite = settings.otohimeFaceJoy
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "んだんだ！おら、おとひめ様の料理のためなら、何度だってやるだ！",
                speakerSprite = settings.taroFaceNormal
            },
            new DialogueNode
            {
                speakerName = "おとひめ",
                text = "その意気込みに免じて、我が一族に伝わる古文書を授けるわ。ここには海に生きる者の究極の技が記されているの。",
                speakerSprite = settings.otohimeFaceJoy
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "なになに……「豊漁の急降下」？……うおぉ！頭の中に、すごい技のイメージが浮かんできただ！",
                speakerSprite = settings.taroFaceSurprise,
                OnNodeStart = () =>
                {
                    if (PlayerAbilityManager.Instance != null)
                    {
                        PlayerAbilityManager.Instance.UnlockAbility("AttackDown");
                    }
                    else
                    {
                        Debug.LogWarning("IngredientEvent: PlayerAbilityManagerのインスタンスが見つかりません。");
                    }
                    var itemGetCanvas = UIManager.Instance?.GetView<ItemGetCanvas>();
                    if (itemGetCanvas != null)
                    {
                        itemGetCanvas.ShowItem(settings.attackDownSprite, "豊漁の急降下", "下攻撃ができるようになる。");
                    }
                }
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "これなら、敵がいっぱい固まってるところも、上から飛び越えていけるっぺ！",
                speakerSprite = settings.taroFaceNormal
            }
        };

        if (dialogueTrigger != null)
        {
            bool missionFinished = false;
            bool missionRewardAvailable = false;
            bool missionStarted = false;
            if (FlagManager.Instance != null)
            {
                missionFinished = FlagManager.Instance.GetFlag(FlagManager.FlagKey.IngredientMissionFinished.ToString(), false);
                missionRewardAvailable = FlagManager.Instance.GetFlag(FlagManager.FlagKey.IngredientMissionRewardAvailable.ToString(), false);
                missionStarted = FlagManager.Instance.GetFlag(FlagManager.FlagKey.IngredientMissionStarted.ToString(), false);
            }

            if (missionFinished)
            {
                dialogueTrigger.SetDialogueNodes(new List<DialogueNode> {
                    new DialogueNode {
                        speakerName = "おとひめ",
                        text = "本当にありがとう、太郎さん。おかげで宴も大成功よ。",
                        speakerSprite = settings.otohimeFaceJoy
                    }
                });
            }
            else if (missionRewardAvailable)
            {
                dialogueTrigger.SetDialogueNodes(missionClearNodes);
                dialogueTrigger.onDialogueEnd = () => {
                    FlagManager.Instance?.SetFlag(FlagManager.FlagKey.IngredientMissionFinished.ToString(), true);
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
            Debug.LogWarning("IngredientEvent: DialogueTriggerコンポーネントが見つかりません。");
        }
    }
}
