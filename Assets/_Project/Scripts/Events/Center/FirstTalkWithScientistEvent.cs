using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueTrigger))]
public class FirstTalkWithScientistEvent : MonoBehaviour
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
            Debug.LogWarning("FirstTalkWithScientistEvent: GameSettingsが見つかりません。");
            return;
        }

        var nodes = new List<DialogueNode>
        {
            new DialogueNode
            {
                speakerName = "科学者",
                text = "……おや。おい、そこの若者。止まれ。死にたいのか？",
                speakerSprite = settings.scientistFaceNormal
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "うわぁ！？びっくりしただ！人だ、やっと人に会えただ！",
                speakerSprite = settings.taroFaceSurprise
            },
            new DialogueNode
            {
                speakerName = "科学者",
                text = "驚いたのはこちらの方だよ。なんだその格好は……？まるで芝居小屋から抜け出してきたような古い身なりをして。このあたりはまだ放射能の汚染がひどい。そんな軽装でうろつくのは自殺行為じゃよ。",
                speakerSprite = settings.scientistFaceConfusion
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "ほうしゃ……？おら、難しいことはわがんね。それよりおじさん、一体何があったんだべ？おら、ほんの数日、竜宮城へ行ってただけなんだ。なのに、村が……みんながいなくなっちまってるだ！",
                speakerSprite = settings.taroFaceConfusion
            },
            new DialogueNode
            {
                speakerName = "科学者",
                text = "竜宮城だと？ハッハッハ！",
                speakerSprite = settings.scientistFaceSurprise
            },
            new DialogueNode
            {
                speakerName = "科学者",
                text = "久しぶりに笑わせてもらったよ。面白い冗談を言う。まるで古いお伽話の「浦島太郎」そのものじゃないか。",
                speakerSprite = settings.scientistFaceJoy
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "お伽話？どうしておらの名前を知っているだ？おらは浜の村のうらしまたろうだっぺ。",
                speakerSprite = settings.taroFaceConfusion
            },
            new DialogueNode
            {
                speakerName = "科学者",
                text = "ハッハッハ！……まあよい。この地獄のような世界じゃ、今さら何が起きても驚きはせん。お主がお伽噺の「浦島太郎」だという体で、話をしてやろう。",
                speakerSprite = settings.scientistFaceJoy
            },
            new DialogueNode
            {
                speakerName = "科学者",
                text = "……「ウラン」という石を知っておるか。人間はな、身の丈に合わない光を手に入れてしまったのじゃ。ウランを原料にした兵器を使い、空を焼き、海を汚し、自ら世界を滅ぼした。それがこの荒廃の正体……核戦争の果てよ。",
                speakerSprite = settings.scientistFaceNormal
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "ウラン……？おらの名前と似てるけど、そんなに恐ろしいもんだったんか。村のみんなは……おっ父もおっ母も、あの子らも、みんなその光に焼かれちまったんだっぺか……？",
                speakerSprite = settings.taroFaceSadness
            },
            new DialogueNode
            {
                speakerName = "科学者",
                text = "……気の毒にな。お主が本当に数百年前の人間なら、もう知っている顔は一人もおらん。残ったのは、絶望に震える生存者と、変異した化け物だけじゃ。",
                speakerSprite = settings.scientistFaceNormal
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "そんな……そんなのってねぇべ……。",
                speakerSprite = settings.taroFaceSadness
            },
            new DialogueNode
            {
                speakerName = "科学者",
                text = "浦島……ウラン……。これは因果の収束じゃな 。お主が海で遊んでいた数百年の間に、地上では我ら人間が愚かな火遊びを繰り返しておったのよ。この灰色の景色こそが、我ら人間が選び取った「未来」という名の成れの果てじゃ。",
                speakerSprite = settings.scientistFaceNormal
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "……。",
                speakerSprite = settings.taroFaceSadness
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "……おら、やっぱり難しいことはよくわがんね。おじさんの言ってることも、この世界のことも、おらにはサッパリだ。でも、ここに突っ立ってても何も始まらねぇ。",
                speakerSprite = settings.taroFaceSerious
            },
            new DialogueNode
            {
                speakerName = "うらしまたろう",
                text = "おら、自分の目でこの先を見てくるだ。村のみんながどこかに逃げてっかもしんねぇし……。とりあえず、いろいろ歩いて回ってみるっぺ！",
                speakerSprite = settings.taroFaceConfidence
            },
            new DialogueNode
            {
                speakerName = "科学者",
                text = "……ふん、おめでたい奴だ。見てくるがいい。だが、この先に広がっておるのは絶望だけじゃぞ。",
                speakerSprite = settings.scientistFaceNormal
            }
        };

        Debug.Log("FirstTalkWithScientistEvent: Setting up dialogue with " + nodes.Count + " nodes.");
        if (dialogueTrigger != null)
        {
            dialogueTrigger.SetDialogueNodes(nodes);
            // 会話終了時にフラグを立てて自身を非アクティブ化
            dialogueTrigger.onDialogueEnd = () => {
                if (FlagManager.Instance != null)
                {
                    FlagManager.Instance.SetFlag(FlagManager.FlagKey.HasTalkedToScientist.ToString(), true);
                }
                gameObject.SetActive(false);
            };
        }
        else
        {
            Debug.LogWarning("FirstTalkWithScientistEvent: DialogueTriggerコンポーネントが見つかりません。");
        }
    }
}
