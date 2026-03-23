using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueTrigger))]
public class GetJetDashEvent : MonoBehaviour
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
            Debug.LogWarning("GetJetDashEvent: GameSettingsが見つかりません。");
            return;
        }

        var nodes = new List<DialogueNode>
        {
            new DialogueNode
            {
                speakerName = " ",
                text = "イカ墨ジェットを手に入れた！",
                speakerSprite = settings.jetDashSprite,
                hasChoices = false,
                onNodeStart = new UnityEngine.Events.UnityEvent()
            }
        };

        // JetDashフラグを立てる処理をノード開始時に追加
        nodes[0].OnNodeStart = () =>
        {
            // アビリティを即時反映
            if (PlayerAbilityManager.Instance != null)
            {
                PlayerAbilityManager.Instance.UnlockAbility("JetDash");
            }
            else
            {
                Debug.LogWarning("GetJetDashEvent: PlayerAbilityManagerのインスタンスが見つかりません。");
            }
            // アイテム取得UI表示（任意: UIManager経由で取得）
            var itemGetCanvas = UIManager.Instance?.GetView<ItemGetCanvas>();
            if (itemGetCanvas != null)
            {
                itemGetCanvas.ShowItem(settings.jetDashSprite, "イカ墨ジェット", "方向キーとジャンプキーを同時に押すことで直線的に移動することができる。");
            }
        };

        // 会話終了時に自身を非アクティブ化する
        if (dialogueTrigger != null)
        {
            dialogueTrigger.onDialogueEnd += () => { gameObject.SetActive(false); };
        }

        if (dialogueTrigger != null)
        {
            dialogueTrigger.SetDialogueNodes(nodes);
        }
        else
        {
            Debug.LogWarning("GetJetDashEvent: DialogueTriggerコンポーネントが見つかりません。");
        }
    }
}
