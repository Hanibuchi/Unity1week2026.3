using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueTrigger))]
public class GetVaccineEvent : MonoBehaviour
{
    private DialogueTrigger dialogueTrigger;

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
                    flag = !FlagManager.Instance.GetFlag(FlagManager.FlagKey.VaccineMissionRewardAvailable.ToString());
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
            Debug.LogWarning("GetVaccineEvent: GameSettingsが見つかりません。");
            return;
        }

        var nodes = new List<DialogueNode>
        {
            new DialogueNode
            {
                speakerName = " ",
                text = "ワクチンを手に入れた！",
                speakerSprite = settings.vaccineSprite, // GameSettingsDataにvaccineSpriteが必要
                hasChoices = false,
                onNodeStart = new UnityEngine.Events.UnityEvent()
            }
        };

        // ワクチンフラグを立てる処理をノード開始時に追加
        nodes[0].OnNodeStart = () =>
        {
            if (FlagManager.Instance != null)
            {
                FlagManager.Instance.SetFlag(FlagManager.FlagKey.VaccineMissionRewardAvailable.ToString(), true);
            }
            else
            {
                Debug.LogWarning("GetVaccineEvent: FlagManagerのインスタンスが見つかりません。");
            }
            // アイテム取得UI表示（任意: UIManager経由で取得）
            var itemGetCanvas = UIManager.Instance?.GetView<ItemGetCanvas>();
            if (itemGetCanvas != null)
            {
                itemGetCanvas.ShowItem(settings.vaccineSprite, "ワクチン", "伝染病を防ぐ貴重なワクチン。");
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
            Debug.LogWarning("GetVaccineEvent: DialogueTriggerコンポーネントが見つかりません。");
        }
    }
}
