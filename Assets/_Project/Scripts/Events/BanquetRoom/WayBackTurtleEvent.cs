using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueTrigger))]
public class WayBackTurtleEvent : MonoBehaviour
{
    private DialogueTrigger dialogueTrigger;

    private void Awake()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();
        SetupDialogue();
    }

    private void SetupDialogue()
    {
        var settings = CommonGameSettings.Settings ?? Resources.Load<GameSettingsData>("GameSettings");
        if (settings == null)
        {
            Debug.LogWarning("WayBackTurtleEvent: GameSettingsが見つかりません。");
            return;
        }

        var nodes = new List<DialogueNode>
        {
            new DialogueNode
            {
                speakerName = "カメ",
                text = "ここは竜宮城の出口です。",
                speakerSprite = settings.kameFaceNormal
            },
            new DialogueNode
            {
                speakerName = "カメ",
                text = "太郎さん、もう帰ってしまうのですか？ここでの生活は、あんな地上よりもずっと快適でしょうに。",
                speakerSprite = settings.kameFaceNormal,
                hasChoices = true,
                choice1Text = "村に帰るだ",
                choice2Text = "まだここにいたいだ",
                choice1NextNodes = new List<DialogueNode>
                {
                    new DialogueNode
                    {
                        speakerName = "うらしまたろう",
                        text = "悪いけど、おら、もう村に帰るだ。長居しすぎると、村のみんなが心配するっぺ。",
                        speakerSprite = settings.taroFaceNormal,
                        hasChoices = false
                    },
                    new DialogueNode
                    {
                        speakerName = "カメ",
                        text = "……そうですか。一度ここを出れば、もう二度と戻ってくることはできませんよ。本当に、後悔しませんか？",
                        speakerSprite = settings.kameFaceNormal,
                        hasChoices = true,
                        choice1Text = "後悔しねぇだ",
                        choice2Text = "やっぱり迷うだ……",
                        choice1NextNodes = new List<DialogueNode>
                        {
                            new DialogueNode
                            {
                                speakerName = "うらしまたろう",
                                text = "おとひめ様の料理が食べられなくなるのは寂しいけど……。おらには、待ってる人がいるんだ。やっぱり帰るだ！",
                                speakerSprite = settings.taroFaceNormal,
                                hasChoices = false
                            }
                        },
                        choice2NextNodes = new List<DialogueNode>
                        {
                            new DialogueNode
                            {
                                speakerName = "カメ",
                                text = "ごゆっくりしていってくださいね。竜宮城はいつまでもあなたを歓迎します。",
                                speakerSprite = settings.kameFaceNormal,
                                hasChoices = false
                            }
                        }
                    }
                },
                choice2NextNodes = new List<DialogueNode>
                {
                    new DialogueNode
                    {
                        speakerName = "カメ",
                        text = "ごゆっくりしていってくださいね。竜宮城はいつまでもあなたを歓迎します。",
                        speakerSprite = settings.kameFaceNormal,
                        hasChoices = false
                    }
                }
            }
        };

        Debug.Log("WayBackTurtleEvent: Setting up dialogue with " + nodes.Count + " nodes.");
        if (dialogueTrigger != null)
        {
            dialogueTrigger.SetDialogueNodes(nodes);
        }
        else
        {
            Debug.LogWarning("WayBackTurtleEvent: DialogueTriggerコンポーネントが見つかりません。");
        }
    }
}
