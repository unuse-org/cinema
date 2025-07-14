using UnityEngine;
using UnityEngine.SceneManagement;

public class ReceiveScore : MonoBehaviour
{
    public int currentScore = 0;
    private int accumulatedScore = 0; // 1.5秒間で蓄積するスコア

    private void Start()
    {
        // 保存されたスコアを取得
        currentScore = PlayerPrefs.GetInt("score", 0);
        Debug.Log($"🟢 スタート時スコア: {currentScore}");

        // シーン変更時のイベント登録
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    // 外部からスコアを加算するメソッド
    public void AddScore(int scoreToAdd)
    {
        accumulatedScore += scoreToAdd;
    }

    // ScoreManager が1.5秒ごとに呼ぶ：蓄積されたスコアを取得して currentScore に加算
    public int FetchAccumulatedScore()
    {
        int delta = accumulatedScore;
        accumulatedScore = 0;
        currentScore += delta;
        return delta;
    }

    // シーンがアンロードされるときに保存
    private void OnSceneUnloaded(Scene current)
    {
        SaveScore();
    }

    private void OnDestroy()
    {
        SaveScore();
    }

    private void OnDisable()
    {
        SaveScore();
    }

    private void SaveScore()
    {
        PlayerPrefs.SetInt("score", currentScore);
        Debug.Log($"💾 スコア保存: {currentScore}");
    }
}
