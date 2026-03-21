using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueTrigger))]
public class GetInsecticideEvent : MonoBehaviour
{
    private DialogueTrigger dialogueTrigger;

    // GameScreen型のメンバを追加
    [SerializeField]
    private GameScreen gameScreen;

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
                    flag = !FlagManager.Instance.GetFlag(FlagManager.FlagKey.ChemicalPlantMissionRewardAvailable.ToString());
                }
                gameObject.SetActive(flag);
            };
        }
    }

    private void SetupDialogue()
    {
        var settings = CommonGameSettings.Settings ?? Resources.Load<GameSettingsData>("GameSettings");
        if (settings == null)
        {
            Debug.LogWarning("GetInsecticideEvent: GameSettingsが見つかりません。");
            return;
        }

        var nodes = new List<DialogueNode>
        {
            new DialogueNode
            {
                speakerName = " ",
                text = "殺虫剤を手に入れた！",
                speakerSprite = settings.insecticideSprite,
                hasChoices = false,
                onNodeStart = new UnityEngine.Events.UnityEvent()
            }
        };

        // 殺虫剤フラグを立てる処理をノード開始時に追加
        nodes[0].OnNodeStart = () =>
        {
            // アビリティやフラグを即時反映
            if (FlagManager.Instance != null)
            {
                FlagManager.Instance.SetFlag(FlagManager.FlagKey.ChemicalPlantMissionRewardAvailable.ToString(), true);
            }
            else
            {
                Debug.LogWarning("GetInsecticideEvent: FlagManagerのインスタンスが見つかりません。");
            }
            // アイテム取得UI表示（任意: UIManager経由で取得）
            var itemGetCanvas = UIManager.Instance?.GetView<ItemGetCanvas>();
            if (itemGetCanvas != null)
            {
                itemGetCanvas.ShowItem(settings.insecticideSprite, "殺虫剤", "強力な殺虫剤。");
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
            Debug.LogWarning("GetInsecticideEvent: DialogueTriggerコンポーネントが見つかりません。");
        }
    }
}
