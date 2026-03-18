using UnityEngine;
using Unity.Cinemachine;
using Unity.VisualScripting;

/// <summary>
/// Cinemachineを制御するシングルトンクラス
/// </summary>
public class CameraController : MonoBehaviour
{
    // シングルトンインスタンス
    public static CameraController Instance { get; private set; }

    [Header("Cinemachine Settings")]
    [SerializeField] private CinemachineCamera virtualCamera;
    
    [Header("Screen Shake Settings")]
    [SerializeField] private CinemachineImpulseSource impulseSource;

    // Confiner2Dのキャッシュ
    private CinemachineConfiner2D confiner2D;

    private void Awake()
    {
        // シングルトンの初期化
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        // シーン切り替えで破棄したくない場合は以下のコメントアウトアウトを解除してください
        // DontDestroyOnLoad(gameObject);

        Initialize();
    }

    private void Initialize()
    {
        if (virtualCamera == null)
        {
            virtualCamera = Object.FindFirstObjectByType<CinemachineCamera>();
        }

        if (virtualCamera != null)
        {
            confiner2D = virtualCamera.GetComponent<CinemachineConfiner2D>();
            
            // Confiner がアタッチされていない場合は追加警告
            if (confiner2D == null)
            {
                Debug.LogWarning("CinemachineVirtualCamera に CinemachineConfiner2D がアタッチされていません。");
            }
        }
        else
        {
            Debug.LogError("CinemachineVirtualCamera が見つかりません。インスペクターから設定するかシーンに配置してください。");
        }

        if (impulseSource == null)
        {
            impulseSource = GetComponent<CinemachineImpulseSource>();
        }
    }

    /// <summary>
    /// Tracking Target (Follow対象) を変更する
    /// </summary>
    /// <param name="target">追従させたいターゲットのTransform</param>
    public void SetTrackingTarget(Transform target)
    {
        if (virtualCamera != null)
        {
            virtualCamera.Follow = target;
        }
    }

    /// <summary>
    /// Cinemachine Confiner 2Dの Bounding Shape 2D を変更する
    /// </summary>
    /// <param name="boundingShape">設定したい Collider2D (PolygonCollider2D や CompositeCollider2D など)</param>
    public void SetBoundingShape(Collider2D boundingShape)
    {
        if (confiner2D != null)
        {
            confiner2D.BoundingShape2D = boundingShape;
            
            // 形状を変更した後は必ずキャッシュを破棄して再計算させる
            confiner2D.InvalidateBoundingShapeCache();
        }
        else
        {
            Debug.LogWarning("CinemachineConfiner2D が設定されていないため、Bounding Shape を変更できません。");
        }
    }

    /// <summary>
    /// Cinemachine Impulse を使って画面を揺らす
    /// </summary>
    /// <param name="force">揺れの強さの倍率（デフォルト1.0）</param>
    public void ShakeScreen(float force = 1f)
    {
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse(force);
        }
        else
        {
            Debug.LogWarning("CinemachineImpulseSource が設定されていないため、画面を揺らすことができません。");
        }
    }

    // [SerializeField] Transform testTransform;
    // public void TestSetTarget()
    // {
    //     SetTrackingTarget(testTransform);
    // }

    // [SerializeField] Collider2D testCollider;
    // public void TestSetBoundingShape()
    // {
    //     SetBoundingShape(testCollider);
    // }
}
