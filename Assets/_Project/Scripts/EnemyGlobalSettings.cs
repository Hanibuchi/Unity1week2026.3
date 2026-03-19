using UnityEngine;

[CreateAssetMenu(fileName = "EnemyGlobalSettings", menuName = "Settings/EnemyGlobalSettings")]
public class EnemyGlobalSettings : ScriptableObject
{
    [Header("Fragment Settings")]
    public string fragmentPrefabPath = "Fragment";
    [Range(0f, 1f)]
    public float fragmentSpawnProbability = 0.5f;
    [Range(0f, 1f)]
    public float healItemSpawnProbability = 0.25f;
    public float minFragmentForce = 5f;
    public float maxFragmentForce = 15f;
    public float fragmentLifetime = 2f;

    [Header("Hit Stop Settings")]
    public float hitStopDuration = 0.05f;
    [Range(0f, 1f)]
    public float hitStopTimeScale = 0.05f;
    public TimeScaleManager.TimeScalePriority hitStopPriority = TimeScaleManager.TimeScalePriority.HitStop;
}