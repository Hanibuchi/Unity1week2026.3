using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueTrigger))]
public class EndingTalkEvent : MonoBehaviour
{
    private DialogueTrigger dialogueTrigger;

    bool accept = false;

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
            Debug.LogWarning("EndingTalkEvent: GameSettingsが見つかりません。");
            return;
        }

        var nodes = new List<DialogueNode>
        {
            new DialogueNode
            {
                speakerName = "科学者",
                text = "……おお、お前さんか。まだこの乾いた大地を彷徨っておったのか 。",
                speakerSprite = settings.scientistFaceNormal,
                OnNodeStart = () => {accept = false;}
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "おじさん、おら、歩いてみただ。でも、どこもかしこも砂とガレキばっかりだったっぺ 。",
                speakerSprite = settings.taroFaceNormal
            },
            new DialogueNode
            {
                speakerName = "科学者",
                text = "そうじゃろうな。この世界がどう見えた？かつての青い海も、緑の山も、人間の身勝手な「光」……ウランの炎に焼かれて消えてしまった 。目にするものすべて、絶望に満ち溢れておったろう 。",
                speakerSprite = settings.scientistFaceNormal
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "おら、難しいことはわがんね 。でも、ここが本当に、おらのいた村のなれの果てなんだべか 。",
                speakerSprite = settings.taroFaceNormal
            },
            new DialogueNode
            {
                speakerName = "科学者",
                text = "……浦島よ。お前さんは竜宮城という「夢」を見てきた 。だが、ここが今の地球の姿、逃れようのない現実じゃ 。",
                speakerSprite = settings.scientistFaceNormal
            },
            new DialogueNode
            {
                speakerName = "科学者",
                text = "どうかね。この救いようのない今の状態を受け入れないか？",
                speakerSprite = settings.scientistFaceNormal,
                hasChoices = true,
                choice1Text = "はい",
                choice1NextNodes = new List<DialogueNode>
                {
                    new DialogueNode
                    {
                        speakerName = "科学者",
                        text = "もうやり残したことは本当にないか？",
                        speakerSprite = settings.scientistFaceNormal,
                        hasChoices = true,
                        choice1Text = "はい",
                        choice1NextNodes = new List<DialogueNode>
                        {
                            new DialogueNode
                            {
                                speakerName = "科学者",
                                text = "……そうか。ならば、もう何も言うまい。",
                                speakerSprite = settings.scientistFaceNormal,
                                OnNodeStart = () =>
                                {
                                    accept = true;
                                }
                            }
                        },
                        choice2Text = "いいえ",
                        choice2NextNodes = new List<DialogueNode>
                        {
                            new DialogueNode
                            {
                                speakerName = "科学者",
                                text = "……そうか。ならば、気が済むまでこの荒野を歩くがよい 。お前さんがこの残酷な現実を飲み込み、心の決着がついたなら……その時はまた、ここへ戻ってくるがいい 。",
                                speakerSprite = settings.scientistFaceNormal
                            }
                        }
                    }
                },
                choice2Text = "いいえ",
                choice2NextNodes = new List<DialogueNode>
                {
                    new DialogueNode
                    {
                        speakerName = "科学者",
                        text = "……そうか。ならば、気が済むまでこの荒野を歩くがよい 。お前さんがこの残酷な現実を飲み込み、心の決着がついたなら……その時はまた、ここへ戻ってくるがいい 。",
                        speakerSprite = settings.scientistFaceNormal
                    }
                }
            }
        };

        Debug.Log("EndingTalkEvent: Setting up dialogue with " + nodes.Count + " nodes.");
        if (dialogueTrigger != null)
        {
            dialogueTrigger.onDialogueEnd = () => { if (accept) EndAccepted(); };
            dialogueTrigger.SetDialogueNodes(nodes);
        }
        else
        {
            Debug.LogWarning("EndingTalkEvent: DialogueTriggerコンポーネントが見つかりません。");
        }
    }


    /// <summary>
    /// 2回の質問に両方「はい」と答えた時、会話の最後で呼ばれる
    /// </summary>
    private void EndAccepted()
    {
        Debug.Log("EndingTalkEvent: 両方はいで会話終了時に呼ばれました。");
        // GameManagerのGameClear状態へ遷移
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeState(GameManager.GameState.GameClear);
        }
    }
}
