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
}
