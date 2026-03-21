using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueTrigger))]
public class FoodWarehouseCompleteEvent : MonoBehaviour
{
    // GameScreen型のメンバを追加
    [SerializeField]
    private GameScreen gameScreen;
    private DialogueTrigger dialogueTrigger;


    private void Awake()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();
        SetupDialogue();

        // GameScreenのonScreenLoadedEventに自身をSetActive(true)する処理を追加
        if (gameScreen != null)
        {
            gameScreen.onScreenLoadedEvent += () =>
            {
                bool flag = true;
                if (FlagManager.Instance != null)
                {
                    flag = !FlagManager.Instance.GetFlag(FlagManager.FlagKey.FoodWarehouseMissionRewardAvailable.ToString());
                }
                gameObject.SetActive(flag);
            };
        }
    }

    private void SetupDialogue()
    {
        var nodes = new List<DialogueNode>
        {
            new DialogueNode
            {
                speakerName = " ",
                text = "食糧庫への道が開かれた！",
                hasChoices = false,
                onNodeStart = new UnityEngine.Events.UnityEvent()
            }
        };

        // フラグを立てる処理をノード開始時に追加
        nodes[0].OnNodeStart = () =>
        {
            // フラグ管理（FlagManager）で食糧庫解放フラグを立てる
            if (FlagManager.Instance != null)
            {
                FlagManager.Instance.SetFlag(FlagManager.FlagKey.FoodWarehouseMissionRewardAvailable.ToString(), true);
            }
            else
            {
                Debug.LogWarning("FoodWarehouseCompleteEvent: FlagManagerのインスタンスが見つかりません。");
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
            Debug.LogWarning("FoodWarehouseCompleteEvent: DialogueTriggerコンポーネントが見つかりません。");
        }
    }
}
