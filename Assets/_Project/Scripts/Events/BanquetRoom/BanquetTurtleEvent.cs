using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueTrigger))]
public class BanquetTurtleEvent : MonoBehaviour
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
            Debug.LogWarning("BanquetTurtleEvent: GameSettingsが見つかりません。");
            return;
        }

        var nodes = new List<DialogueNode>
        {
            new DialogueNode
            {
                speakerName = "カメ",
                text = "太郎さん、料理はお口に合いましたか？",
                speakerSprite = settings.kameFaceNormal
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "おら、こんなにおいしいもの食ったの初めてだ。村の皆にも食わせてやりてぇなぁ。",
                speakerSprite = settings.taroFaceNormal
            },
            new DialogueNode
            {
                speakerName = "カメ",
                text = "……村、ですか。まあ、今はそのことは忘れて、この竜宮城で心ゆくまでくつろいでいってください。",
                speakerSprite = settings.kameFaceNormal
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "んだな。カメさん、ありがとよ。",
                speakerSprite = settings.taroFaceNormal
            }
        };

        Debug.Log("BanquetTurtleEvent: Setting up dialogue with " + nodes.Count + " nodes.");
        if (dialogueTrigger != null)
        {
            dialogueTrigger.SetDialogueNodes(nodes);
        }
        else
        {
            Debug.LogWarning("BanquetTurtleEvent: DialogueTriggerコンポーネントが見つかりません。");
        }
    }
}
