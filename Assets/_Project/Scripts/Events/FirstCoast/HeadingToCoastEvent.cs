using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueTrigger))]
public class HeadingToCoastEvent : MonoBehaviour
{
    private DialogueTrigger dialogueTrigger;

    private void Awake()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();
        SetupDialogue();
        // 会話終了時に自身を非アクティブ化
        if (dialogueTrigger != null)
        {
            dialogueTrigger.onDialogueEnd += () => gameObject.SetActive(false);
        }
    }

    private void SetupDialogue()
    {
        var settings = CommonGameSettings.Settings ?? Resources.Load<GameSettingsData>("GameSettings");
        if (settings == null)
        {
            Debug.LogWarning("HeadingToCoastEvent: GameSettingsが見つかりません。");
            return;
        }

        var nodes = new List<DialogueNode>
        {
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "今日もいい天気だぁ。",
                speakerSprite = settings.taroFaceNormal
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "暇だからちょっと海岸を散歩するべ。",
                speakerSprite = settings.taroFaceNormal
            }
        };

        Debug.Log("HeadingToCoastEvent: Setting up dialogue with " + nodes.Count + " nodes.");
        if (dialogueTrigger != null)
        {
            dialogueTrigger.SetDialogueNodes(nodes);
        }
        else
        {
            Debug.LogWarning("HeadingToCoastEvent: DialogueTriggerコンポーネントが見つかりません。");
        }
    }
}
