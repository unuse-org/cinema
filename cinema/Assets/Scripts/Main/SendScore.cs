using UnityEngine;

public class SendScore : MonoBehaviour
{
    [Header("スコアを受け取る側のスクリプト")]
    [SerializeField] private ReceiveScore receiveScore;

    private int baseScore = 10;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 1f)
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
