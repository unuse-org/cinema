using UnityEngine;
using System.Collections;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] private Animator targetAnimator;   // アニメーションを持つオブジェクト
    [SerializeField] private string animationName;      // 再生するアニメーション名（Animator内のステート名）

    private bool isPlaying = false;

    void Start()
    {
        if (targetAnimator == null)
        {
            Debug.LogError("Animator が設定されていません。");
            return;
        }

        // 起動時は完全に停止状態（最初のフレームで止める）
        targetAnimator.Play(animationName, 0, 0f);
        targetAnimator.speed = 0;
    }

    void Update()
    {
        // デバッグ用：スペースキーでアニメーション全再生
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayFullAnimation();
        }
    }

    /// <summary>
    /// アニメーションを全て再生
    /// </summary>
    public void PlayFullAnimation()
    {
        if (isPlaying || targetAnimator == null) return;

        isPlaying = true;
        targetAnimator.speed = 1;                    // 再生開始
        targetAnimator.Play(animationName, 0, 0f);   // 先頭から再生
    }
}
