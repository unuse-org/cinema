using UnityEngine;
using TMPro;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI PopUpText;
    [SerializeField] private CanvasGroup PopUpCanvasGroup;  // CanvasGroupを追加

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
        
        // 初期設定で透明度を1にしておく
        if (PopUpCanvasGroup != null)
        {
            PopUpCanvasGroup.alpha = 1f;
        }
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

                pupup(accumulatedScore);

                // リセット
                accumulatedScore = 0;
            }
        }
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "<sprite name=\"money_satsutaba\">"+score.ToString();
            scoreText.characterSpacing = -2f; 
        }
    }

    public void SaveScore(int score)
    {
        PlayerPrefs.SetInt("score", score);
        PlayerPrefs.Save();
    }

   private void pupup(int accumulatedScore)
    {
        // ポップアップテキストを設定
        PopUpText.text = "+" + "<sprite name=\"money_satsutaba\">" + accumulatedScore;

        // 透明度を元に戻す（新しいポップアップ時）
        if (PopUpCanvasGroup != null)
        {
            PopUpCanvasGroup.alpha = 1f;
        }

        // フェードアウトを実行
        StartFadeOut();
    }

    private void StartFadeOut()
    {
        if (PopUpCanvasGroup != null)
        {
            // フェードアウトを毎フレーム処理する
            StartCoroutine(FadeOut());
        }
    }
    private IEnumerator FadeOut()
    {
        
        float elapsedTime = 0f;
        float startAlpha = PopUpCanvasGroup.alpha;
        float endAlpha = 0f;
        float fadeSpeed = 2f;

        // 透明度を徐々に下げる
        while (elapsedTime < 1f)  // 1秒以内で透明度を下げる
        {
            elapsedTime += Time.deltaTime * fadeSpeed;
            PopUpCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime);
            yield return null;
        }

        // 完全に透明に設定
        PopUpCanvasGroup.alpha = endAlpha;
    }
}

