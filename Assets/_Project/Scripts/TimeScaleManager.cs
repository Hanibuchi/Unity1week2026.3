using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimeScaleManager : MonoBehaviour
{
    public enum TimeScalePriority
    {
        Default = 0,
        HitStop = 10,
        Menu = 20,
        System = 100
    }

    public static TimeScaleManager Instance { get; private set; }

    private class TimeScaleRequest
    {
        public float Scale;
        public int Priority;
        public object Owner;
    }

    private List<TimeScaleRequest> requests = new List<TimeScaleRequest>();
    
    // TimeScale変更時にPhysicsの更新設定もあわせて調整するためのデフォルト値
    private float defaultFixedDeltaTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            defaultFixedDeltaTime = Time.fixedDeltaTime;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// タイムスケールの変更を要求します。既に同じownerからの要求がある場合は上書きされます。
    /// 優先度(priority)が最も高い要求が現在のTime.timeScaleとして適用されます。
    /// </summary>
    public void SetTimeScale(float scale, int priority, object owner)
    {
        var existingRequest = requests.FirstOrDefault(r => r.Owner == owner);
        if (existingRequest != null)
        {
            existingRequest.Scale = scale;
            existingRequest.Priority = priority;
        }
        else
        {
            requests.Add(new TimeScaleRequest { Scale = scale, Priority = priority, Owner = owner });
        }

        ApplyHighestPriorityScale();
    }

    /// <summary>
    /// 対象ownerのタイムスケール変更要求を取り消します。
    /// 用が済んだらリストから削除し、次に優先度が高い要求（なければ等倍 1.0）に戻します。
    /// </summary>
    public void RemoveRequest(object owner)
    {
        int removedCount = requests.RemoveAll(r => r.Owner == owner);
        if (removedCount > 0)
        {
            ApplyHighestPriorityScale();
        }
    }

    private void ApplyHighestPriorityScale()
    {
        if (requests.Count == 0)
        {
            Time.timeScale = 1.0f;
            Time.fixedDeltaTime = defaultFixedDeltaTime;
            return;
        }

        // 最も優先度の高い要求を取得
        var highestRequest = requests.OrderByDescending(r => r.Priority).First();
        
        Time.timeScale = highestRequest.Scale;
        
        // 物理演算(FixedUpdate)のステップもTimeScaleに合わせて調整することでカクつきを防ぎます
        // TimeScaleが0の時はエラーを防ぐため調整しないか、または0をかける（必要に応じて変更）
        Time.fixedDeltaTime = defaultFixedDeltaTime * (Time.timeScale == 0 ? 1f : Time.timeScale);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Time.timeScale = 1.0f;
            Time.fixedDeltaTime = defaultFixedDeltaTime;
        }
    }
}
