using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("スコアを受け取る側のスクリプト")]
    [SerializeField] private ReceiveScore receiveScore;

    private int score;

    
    // スコアを保存するメソッド
    public void SaveScore(int score)
    {
        // PlayerPrefsにスコアを保存
        PlayerPrefs.SetInt("score", score);
        PlayerPrefs.Save();
    }

    void Start()
    {
        
    }
    void Update()
    {
        try
        {
            score = receiveScore.currentScore;
        }
        catch (System.NullReferenceException e)
        {
            //Debug.LogError($"❌ receiveScore が null、または currentScore にアクセスできません: {e.Message}");
        }

        // UIに表示
        scoreText.text = "スコア" + score.ToString();
    }

}
