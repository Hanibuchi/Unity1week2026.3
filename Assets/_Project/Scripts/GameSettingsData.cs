using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettings")]
public class GameSettingsData : ScriptableObject
{
    [Header("UI Sounds")]
    public AudioClip uiSelectSound;
    public AudioClip uiDecideSound;

    [Header("Character Faces (Taro)")]
    public Sprite taroFaceNormal;
    public Sprite taroFaceJoy;
    public Sprite taroFaceAnger;
    public Sprite taroFaceSadness;
    public Sprite taroFaceConfusion;
    public Sprite taroFaceSurprise;
    public Sprite taroFaceDisgust;
    public Sprite taroFaceDamage;
    public Sprite taroFaceConfidence;
    public Sprite taroFaceSerious;
    public Sprite taroFaceOld;

    public Sprite futureTaroFaceNormal;
    public Sprite futureTaroFaceJoy;

    [Header("Character Faces (Children)")]
    public Sprite childFaceConfidence;
    public Sprite childFaceSurprise;
    public Sprite childFaceDamage;

    [Header("Character Faces (Kame)")]
    public Sprite kameFaceNormal;
    public Sprite kameFaceJoy;
    public Sprite kameFaceAnger;
    public Sprite kameFaceSadness;
    public Sprite kameFaceConfusion;
    public Sprite kameFaceSurprise;
    public Sprite kameFaceDisgust;
    public Sprite kameFaceDamage;
    public Sprite kameFaceConfidence;

    [Header("Character Faces (Otohime)")]
    public Sprite otohimeFaceNormal;
    public Sprite otohimeFaceJoy;
    public Sprite otohimeFaceAnger;
    public Sprite otohimeFaceSadness;
    public Sprite otohimeFaceConfusion;
    public Sprite otohimeFaceSurprise;
    public Sprite otohimeFaceDisgust;
    public Sprite otohimeFaceDamage;
    public Sprite otohimeFaceConfidence;
    public Sprite otohimeFaceSerious;
    public Sprite otohimeFaceDisbelief;
    public Sprite otohimeFaceCute;

    [Header("Character Faces (Red Kame)")]
    public Sprite redKameFaceNormal;
    public Sprite redKameFaceJoy;
    public Sprite redKameFaceAnger;
    public Sprite redKameFaceSadness;
    public Sprite redKameFaceConfusion;
    public Sprite redKameFaceSurprise;
    public Sprite redKameFaceDisgust;
    public Sprite redKameFaceDamage;
    public Sprite redKameFaceConfidence;

    [Header("Pet Icons")]
    public Sprite petIconNormal;
    public Sprite petIconJoy;

    [Header("Scientist Icons")]
    public Sprite scientistFaceNormal;
    public Sprite scientistFaceJoy;
    public Sprite scientistFaceAnger;
    public Sprite scientistFaceSadness;
    public Sprite scientistFaceConfusion;
    public Sprite scientistFaceSurprise;
    public Sprite scientistFaceDisgust;
    public Sprite scientistFaceDamage;
    public Sprite scientistFaceConfidence;

    [Header("Character Faces (Modern People A)")]
    public Sprite modernPeopleAFaceNormal;
    public Sprite modernPeopleAFaceJoy;
    public Sprite modernPeopleAFaceFlustered;
    public Sprite modernPeopleAFaceSadness;
    public Sprite modernPeopleAFaceWorry;
    public Sprite modernPeopleAFaceSurprise;
    public Sprite modernPeopleAFaceSerious;

    [Header("Character Faces (Modern People B)")]
    public Sprite modernPeopleBFaceNormal;
    public Sprite modernPeopleBFaceJoy;
    public Sprite modernPeopleBFaceFlustered;
    public Sprite modernPeopleBFaceSadness;
    public Sprite modernPeopleBFaceWorry;
    public Sprite modernPeopleBFaceSurprise;
    public Sprite modernPeopleBFaceSerious;

    [Header("Character Faces (Modern People C)")]
    public Sprite modernPeopleCFaceNormal;
    public Sprite modernPeopleCFaceJoy;
    public Sprite modernPeopleCFaceFlustered;
    public Sprite modernPeopleCFaceSadness;
    public Sprite modernPeopleCFaceWorry;
    public Sprite modernPeopleCFaceSurprise;
    public Sprite modernPeopleCFaceSerious;

    [Header("Character Faces (Modern People D)")]
    public Sprite modernPeopleDFaceNormal;
    public Sprite modernPeopleDFaceJoy;
    public Sprite modernPeopleDFaceFlustered;
    public Sprite modernPeopleDFaceSadness;
    public Sprite modernPeopleDFaceWorry;
    public Sprite modernPeopleDFaceSurprise;
    public Sprite modernPeopleDFaceSerious;

    [Header("Character Faces (Modern People E)")]
    public Sprite modernPeopleEFaceNormal;
    public Sprite modernPeopleEFaceJoy;
    public Sprite modernPeopleEFaceFlustered;
    public Sprite modernPeopleEFaceSadness;
    public Sprite modernPeopleEFaceWorry;
    public Sprite modernPeopleEFaceSurprise;
    public Sprite modernPeopleEFaceSerious;

    [Header("Item Sprites")]
    [Tooltip("イカ墨ジェットのアイテム画像")]
    public Sprite jetDashSprite;
    [Tooltip("攻撃ダウンのアイテム画像")]
    public Sprite attackDownSprite;
    [Tooltip("攻撃力アップのアイテム画像")]
    public Sprite increaseAttackSprite;

    [Tooltip("殺虫剤のアイテム画像")]
    public Sprite insecticideSprite;
    [Tooltip("ワクチンのアイテム画像")]
    public Sprite vaccineSprite;


}
