using System.Collections;
using UnityEngine;

/// <summary>
/// 画面遷移を実行するコンポーネント。
/// Collider2D (IsTrigger = true) にアタッチして使用します。
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ScreenTransitionGate : MonoBehaviour
{
    [Header("Screen Settings")]
    [Tooltip("現在の画面（遷移元のGameScreenオブジェクト）")]
    [SerializeField] private GameScreen currentScreen;
    [Tooltip("次の画面（遷移先のGameScreenオブジェクト）")]
    [SerializeField] private GameScreen nextScreen;

    [Header("Player Settings")]
    [Tooltip("遷移後のプレイヤーのスポーン位置（空のGameObject等）")]
    [SerializeField] private Transform nextPlayerSpawnPoint;

    [Header("Transition Settings")]
    [Tooltip("暗転にかかる時間（リアルタイム秒）")]
    [SerializeField] private float fadeOutDuration = 0.5f;
    [Tooltip("明転にかかる時間（リアルタイム秒）")]
    [SerializeField] private float fadeInDuration = 0.5f;
    [Space]
    [Tooltip("遷移時に再生するSE")]
    [SerializeField] private AudioClip transitionSE;

    private bool _isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // すでに遷移処理中であれば多重実行を防ぐ
        if (_isTransitioning) return;

        // Playerタグを持つオブジェクトと衝突した場合に遷移開始
        if (other.CompareTag("Player"))
        {
            StartCoroutine(TransitionRoutine(other.transform.parent));
        }
    }

    /// <summary>
    /// 外部から遷移を発生させるメソッド
    /// </summary>
    public void StartTransition(Transform playerTransform)
    {
        if (!_isTransitioning)
        {
            StartCoroutine(TransitionRoutine(playerTransform));
        }
    }

    private IEnumerator TransitionRoutine(Transform playerTransform)
    {
        _isTransitioning = true;

        if (transitionSE != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySE(transitionSE);
        }

        // 1. ゲーム時間を停止 (TimeScale = 0)
        if (TimeScaleManager.Instance != null)
        {
            TimeScaleManager.Instance.SetTimeScale(0f, (int)TimeScaleManager.TimeScalePriority.System, this);
        }

        // 2. イベント開始（暗転フェードアニメーション）
        if (UIManager.Instance != null)
        {
            UIManager.Instance.Show<ScreenTransitionUI>();
        }

        // タイムスケールが0のため、WaitForSecondsRealtimeを使用する
        yield return new WaitForSecondsRealtime(fadeOutDuration);

        // 3. 画面のアンロード・ロード切り替え
        if (currentScreen != null)
        {
            currentScreen.OnScreenUnloaded();
        }

        if (nextScreen != null)
        {
            nextScreen.OnScreenLoaded();
        }

        // 4. プレイヤーの位置・向きを強制移動
        if (nextPlayerSpawnPoint != null)
        {
            playerTransform.position = nextPlayerSpawnPoint.position;
            playerTransform.rotation = nextPlayerSpawnPoint.rotation;

            // ルートオブジェクトのRigidbody2Dのvelocityを0にリセット
            Rigidbody2D rb = playerTransform.GetComponent<Rigidbody2D>();
            if (rb == null && playerTransform.root != null)
            {
                rb = playerTransform.root.GetComponent<Rigidbody2D>();
            }
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }

        // --- カメラなどを更新させるため、少しだけ時間を動かす ---
        if (TimeScaleManager.Instance != null)
        {
            TimeScaleManager.Instance.RemoveRequest(this);
        }
        yield return new WaitForSecondsRealtime(0.1f); // 0.1秒進めて各種Update（Cinemachine等）をしっかり走らせる
        if (TimeScaleManager.Instance != null)
        {
            TimeScaleManager.Instance.SetTimeScale(0f, (int)TimeScaleManager.TimeScalePriority.System, this);
        }
        // --------------------------------------------------------

        // 5. アニメーション開始（明転フェードアニメーション）
        if (UIManager.Instance != null)
        {
            UIManager.Instance.Hide<ScreenTransitionUI>();
        }

        // 明転が完了するまで待機
        yield return new WaitForSecondsRealtime(fadeInDuration);

        // 6. 時間の再開
        if (TimeScaleManager.Instance != null)
        {
            TimeScaleManager.Instance.RemoveRequest(this);
        }

        _isTransitioning = false;
    }

    /// <summary>
    /// エディタ上で、プレイヤーの遷移先が分かりやすいようにGizmoを描画する。
    /// （Sceneビューのみで表示されます）
    /// </summary>
    private void OnDrawGizmos()
    {
        if (nextPlayerSpawnPoint != null)
        {
            // 次のスポーン位置を緑色の球体で表示
            Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
            Gizmos.DrawSphere(nextPlayerSpawnPoint.position, 0.3f);

            // このゲートからスポーン位置への接続線を引く
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, nextPlayerSpawnPoint.position);
        }
    }

    public void TestShowScreenTransitionUI()
    {
        UIManager.Instance.Show<ScreenTransitionUI>();
    }
    
    public void TestHideScreenTransitionUI()
    {
        UIManager.Instance.Hide<ScreenTransitionUI>();
    }
}
