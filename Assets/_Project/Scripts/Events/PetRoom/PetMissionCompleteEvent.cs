using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueTrigger))]
public class PetMissionCompleteEvent : MonoBehaviour
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
            SetupDialogue();
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
        SetupDialogue();
    }

    private void SetupDialogue()
    {
        var settings = CommonGameSettings.Settings ?? Resources.Load<GameSettingsData>("GameSettings");
        if (settings == null)
        {
            Debug.LogWarning("PetMissionCompleteEvent: GameSettingsが見つかりません。");
            return;
        }

        // 報酬受け取り済みなら自身を非表示
        if (FlagManager.Instance != null && FlagManager.Instance.GetFlag(FlagManager.FlagKey.PetMissionRewardAvailable.ToString(), false))
        {
            gameObject.SetActive(false);
            return;
        }

        var nodes = new List<DialogueNode>
        {
            new DialogueNode
            {
                speakerName = "熱帯魚",
                text = "プクプク……！（助けてくれてありがとう！）",
                speakerSprite = settings.petIconJoy,
                OnNodeStart = () => {
                    // フラグを立てる
                    FlagManager.Instance?.SetFlag(FlagManager.FlagKey.PetMissionRewardAvailable.ToString(), true);
                    // アイテム取得UI表示（任意: UIManager経由で取得）
                    var itemGetCanvas = UIManager.Instance?.GetView<ItemGetCanvas>();
                    if (itemGetCanvas != null)
                    {
                        itemGetCanvas.ShowItem(settings.petIconJoy, "熱帯魚", "おとひめのペット。おとひめといつも一緒に寝ている。");
                    }
                }
            }
        };

        if (dialogueTrigger != null)
        {
            dialogueTrigger.SetDialogueNodes(nodes);
        }
        else
        {
            Debug.LogWarning("PetMissionCompleteEvent: DialogueTriggerコンポーネントが見つかりません。");
        }
    }
}
