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
}
