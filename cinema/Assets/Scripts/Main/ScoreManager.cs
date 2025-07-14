using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("スコアを受け取る側のスクリプト")]
    [SerializeField] private ReceiveScore receiveScore;

    [Header("効果音")]
    [SerializeField] private AudioSource seAudioSource; // SEを再生するAudioSource
    [SerializeField] private AudioClip seClip; // 加算時のSE

    private int score = 0;
    private int accumulatedScore = 0; // 1.5秒間の合計スコア

    private float timer = 0f;
    private const float interval = 1.5f;

    void Start()
    {
        score = PlayerPrefs.GetInt("score", 0);
        UpdateScoreText();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (receiveScore != null)
        {
            // 受け取ったスコアを加算（このタイミングで加算せず蓄積する）
            accumulatedScore += receiveScore.FetchAccumulatedScore(); // 🔄
        }

        if (timer >= interval)
        {
            timer = 0f;

            if (accumulatedScore > 0)
            {
                score += accumulatedScore;
                SaveScore(score);
                UpdateScoreText();

                // SEを再生
                if (seAudioSource != null && seClip != null)
                {
                    seAudioSource.PlayOneShot(seClip);
                }

                // リセット
                accumulatedScore = 0;
            }
        }
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "スコア " + score.ToString();
        }
    }

    public void SaveScore(int score)
    {
        PlayerPrefs.SetInt("score", score);
        PlayerPrefs.Save();
    }
}

