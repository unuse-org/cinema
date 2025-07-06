using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    // スコアを保存するメソッド
    public void SaveScore(int score)
    {
        // PlayerPrefsにスコアを保存
        PlayerPrefs.SetInt("score", score);
        PlayerPrefs.Save();
    }

    void Start()
    {
        // PlayerPrefsから"score"を取得（なければ0）
        int score = PlayerPrefs.GetInt("score", 0);

        // UIに表示
        scoreText.text = "人数" + score.ToString();
    }
}
