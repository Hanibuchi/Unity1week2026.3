using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 複数のGameObject（敵など）を監視し、すべてDestroyされた時にイベントを発火する汎用クラス
/// </summary>
public class GroupDestroyTracker : MonoBehaviour
{
    public UnityEvent onAllDestroyed = new UnityEvent();
    
    private List<GameObject> trackedObjects = new List<GameObject>();
    private bool isTracking = false;

    /// <summary>
    /// トラッカー用のGameObjectを新規作成し、追跡を開始する便利メソッド
    /// </summary>
    public static GroupDestroyTracker Create(List<GameObject> objectsToTrack)
    {
        GameObject go = new GameObject("GroupDestroyTracker");
        GroupDestroyTracker tracker = go.AddComponent<GroupDestroyTracker>();
        tracker.TrackGroup(objectsToTrack);
        return tracker;
    }

    public void TrackGroup(List<GameObject> objectsToTrack)
    {
        trackedObjects.Clear();
        if (objectsToTrack != null)
        {
            foreach (var obj in objectsToTrack)
            {
                // まだDestroyされていない要素だけ追加
                if (obj != null)
                {
                    trackedObjects.Add(obj);
                }
            }
        }
        
        if (trackedObjects.Count > 0)
        {
            isTracking = true;
        }
        else
        {
            // 追跡対象がない場合は即時発火
            TriggerAllDestroyed();
        }
    }

    private void Update()
    {
        if (!isTracking) return;

        // DestroyされたGameObjectはUnity上nullとして扱われるため、nullになった要素をリストから除去
        trackedObjects.RemoveAll(obj => obj == null);

        // すべて除去されたら全滅
        if (trackedObjects.Count == 0)
        {
            TriggerAllDestroyed();
        }
    }

    private void TriggerAllDestroyed()
    {
        isTracking = false;
        onAllDestroyed?.Invoke();
        
        // 用済みになったら自身（トラッカー用オブジェクト）を破棄する
        Destroy(gameObject);
    }
}
