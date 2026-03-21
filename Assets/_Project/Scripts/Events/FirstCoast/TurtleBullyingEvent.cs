using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueTrigger))]
public class TurtleBullyingEvent : MonoBehaviour
{
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private Transform[] spawnPoints;

    [Header("次の海イベント")]
    [SerializeField] private UnderTheSeaEvent underTheSeaEvent;

    private DialogueTrigger dialogueTrigger;

    private void Awake()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();
        SetupDialogue();

        if (dialogueTrigger != null)
        {
            dialogueTrigger.onDialogueEnd += OnDialogueEnded;
        }
    }

    private void OnDestroy()
    {
        if (dialogueTrigger != null)
        {
            dialogueTrigger.onDialogueEnd -= OnDialogueEnded;
        }
    }

    private void OnDialogueEnded()
    {
        if (dialogueTrigger != null)
        {
            dialogueTrigger.onDialogueEnd -= OnDialogueEnded;
        }
        List<GameObject> spawnedEnemies = new List<GameObject>();

        // プレハブをスポーン
        if (prefabToSpawn != null && spawnPoints != null)
        {
            foreach (var point in spawnPoints)
            {
                if (point != null)
                {
                    var enemy = Instantiate(prefabToSpawn, point.position, point.rotation);
                    spawnedEnemies.Add(enemy);
                }
            }
        }

        // 敵の全滅を監視するトラッカーを生成して仕掛ける
        if (spawnedEnemies.Count > 0)
        {
            var tracker = GroupDestroyTracker.Create(spawnedEnemies);
            tracker.onAllDestroyed.AddListener(() =>
            {
                // 敵全滅時にアニメーションのトリガーを設定
                var animator = GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetTrigger("ButtleEnd");
                }
                else
                {
                    Debug.LogWarning("TurtleBullyingEvent: Animatorコンポーネントが見つかりません。");
                }

                // 敵全滅時にダイアログを表示
                ShowAfterBattleDialogue();
            });
        }

        // 会話終了時にアニメーションのトリガーを設定する
        var anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("ButtleBegin");
        }
        else
        {
            Debug.LogWarning("TurtleBullyingEvent: Animatorコンポーネントが見つかりません。");
        }
    }

    // 戦闘後のダイアログを表示
    private void ShowAfterBattleDialogue()
    {
        var settings = CommonGameSettings.Settings ?? Resources.Load<GameSettingsData>("GameSettings");
        if (dialogueTrigger == null || settings == null) return;
        var nodes = new List<DialogueNode>
        {
            new DialogueNode
            {
                speakerName = "子供たち",
                text = "うわぁぁん！たろう、力が強すぎるよ！",
                speakerSprite = settings.childFaceDamage
            },
            new DialogueNode
            {
                speakerName = "子供たち",
                text = "覚えとけよ！母ちゃんに言いつけてやるからな！",
                speakerSprite = settings.childFaceSurprise
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "やれやれ、行っちまっただ。……おーい、カメさん。怪我はねぇか？怖かったっぺな、もう大丈夫だ。",
                speakerSprite =  settings.taroFaceNormal
            },
            new DialogueNode
            {
                speakerName = "カメ",
                text = "……助かりました。ありがとうございます、太郎さん。",
                speakerSprite = settings.kameFaceNormal
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "うわぁぁ！？カメがしゃべっただ！おら、ついに暑さでおかしくなったんか！？",
                speakerSprite = settings.taroFaceSurprise
            },
            new DialogueNode
            {
                speakerName = "カメ",
                text = "驚かないでください。私は竜宮城の案内役を務める者です。自分より人数の多い相手に、勝算も考えず飛び込んでくるなんて……。おかげで命拾いしました。",
                speakerSprite = settings.kameFaceSurprise
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "りゅうぐうじょう……？おら、難しいことはわがんね。でも、困ってるやつがいんなら、助けねばなんねぇだ！",
                speakerSprite = settings.taroFaceConfusion
            },
            new DialogueNode
            {
                speakerName = "カメ",
                text = "命を救っていただいたお礼に、海の底にある極楽、竜宮城へご案内しましょう。",
                speakerSprite = settings.kameFaceJoy
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "海の底に城があるんだべか！そこに行けば、お腹いっぱい美味しいものが食べられるだか？",
                speakerSprite = settings.taroFaceSurprise
            },
            new DialogueNode
            {
                speakerName = "カメ",
                text = "ええ。あなたの想像もつかないような歓迎が待っていますよ。さあ、私の背中に。",
                speakerSprite = settings.kameFaceConfidence
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "んだか！なら、おら行ってみるだ！カメさん、よろしく頼むっぺ！",
                speakerSprite = settings.taroFaceConfidence
            }
        };
        dialogueTrigger.SetDialogueNodes(nodes);
        dialogueTrigger.SetAdjustPlayerPosition(true);
        dialogueTrigger.onDialogueEnd += OnAfterKameDialogueEnd;
        dialogueTrigger.Interact();

    // カメとの会話終了後の処理
    void OnAfterKameDialogueEnd()
    {
        dialogueTrigger.onDialogueEnd -= OnAfterKameDialogueEnd;
        if (underTheSeaEvent != null)
        {
            underTheSeaEvent.StartEvent();
        }
        else
        {
            Debug.LogWarning("TurtleBullyingEvent: underTheSeaEventが設定されていません。");
        }
    }
    }


    private void SetupDialogue()
    {
        // CommonGameSettings.Settings がまだ初期化されていない場合は直接読み込む
        var settings = CommonGameSettings.Settings ?? Resources.Load<GameSettingsData>("GameSettings");
        if (settings == null)
        {
            Debug.LogWarning("TurtleBullyingEvent: GameSettingsが見つかりません。");
            return;
        }

        var nodes = new List<DialogueNode>
        {
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "あ、あそこで子供たちが集まって何してんだべ？",
                speakerSprite = settings.taroFaceSurprise
            },
            new DialogueNode
            {
                speakerName = "子供たち",
                text = "おい、そのカメ貸せよ！おもしろいことして遊ぶんだ。",
                speakerSprite = settings.childFaceConfidence
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "これこれ、おめぇら！そんなことしちゃいけねぇ。そのカメだっておめぇらと同じで、痛いのは嫌なんだべ。すぐ放してやるだ！",
                speakerSprite = settings.taroFaceAnger
            },
            new DialogueNode
            {
                speakerName = "子供たち",
                text = "なんだぁ？たろう、邪魔する気かよ！",
                speakerSprite = settings.childFaceConfidence
            },
            new DialogueNode
            {
                speakerName = "子供たち",
                text = "生意気だぞ！みんな、やっちまえ！ボコボコにしてやる！",
                speakerSprite = settings.childFaceConfidence
            }
        };

        Debug.Log("TurtleBullyingEvent: Setting up dialogue with " + nodes.Count + " nodes.");
        if (dialogueTrigger != null)
        {
            dialogueTrigger.SetDialogueNodes(nodes);
        }
        else
        {
            Debug.LogWarning("TurtleBullyingEvent: DialogueTriggerコンポーネントが見つかりません。");
        }
        dialogueTrigger.SetDialogueNodes(nodes);
    }
}
