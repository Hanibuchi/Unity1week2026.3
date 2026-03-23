using UnityEngine;

/// <summary>
/// 画面遷移時に画面を暗転（フェード）させるためのUIクラス
/// </summary>
[RequireComponent(typeof(Animator))]
public class EndingUI : UIView
{
    [SerializeField] private float hideDuration = 0.5f;
    
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// UIを表示し、"Show"のトランジション（暗転等）アニメーションを再生します。
    /// </summary>
    public override void Show()
    {
        // 先にアクティブにしてからアニメーションをトリガーする
        _isVisible = true;
        gameObject.SetActive(true);

        if (animator != null)
        {
            animator.SetTrigger("Show");
        }
    }

    /// <summary>
    /// "Hide"のアニメーション（明転等）を再生します。
    /// 指定された時間 (hideDuration) 経過後にオブジェクトを非表示にします。
    /// </summary>
    public override void Hide()
    {
        _isVisible = false;
        
        if (animator != null)
        {
            animator.SetTrigger("Hide");
            StartCoroutine(DeactivateAfterDelay(hideDuration));
        }
        else
        {
            // Animatorがない場合のフォールバックとして即座に非表示
            DeactivateComplete();
        }
    }

    private System.Collections.IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        DeactivateComplete();
    }

    /// <summary>
    /// アニメーション終了後に呼び出して、オブジェクトを非表示にします。
    /// </summary>
    private void DeactivateComplete()
    {
        gameObject.SetActive(false);
    }
}