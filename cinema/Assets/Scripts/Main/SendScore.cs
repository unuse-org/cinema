using UnityEngine;

public class SendScore : MonoBehaviour
{
    [Header("スコアを受け取る側のスクリプト")]
    [SerializeField] private ReceiveScore receiveScore;

    [Header("VideoPlayerManager 参照")]
    [SerializeField] private VideoPlayerManager videoPlayerManager; // VideoPlayerManager を参照する

    private int baseScore = 100;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        // 動画が再生されており、accidentActive が true のときだけスコアを加算
        if (timer >= 1f && videoPlayerManager != null && !videoPlayerManager.accidentActive && videoPlayerManager.isVideoPlaying)
        {
            timer = 0f;
            if (receiveScore != null)
            {
                receiveScore.AddScore(baseScore);
                //Debug.Log($"📤 スコア {baseScore} を送信しました");
            }
            else
            {
                Debug.LogWarning("⚠️ receiveScore が設定されていません！");
            }
        }
    }
}
