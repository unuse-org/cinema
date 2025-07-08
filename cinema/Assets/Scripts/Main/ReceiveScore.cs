using UnityEngine;
using UnityEngine.SceneManagement;

public class ReceiveScore : MonoBehaviour
{
    public int currentScore = 0;

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
        currentScore += scoreToAdd;
        Debug.Log($"スコア加算: {scoreToAdd} → 現在のスコア: {currentScore}");
    }

    // シーンがアンロードされるときに保存
    private void OnSceneUnloaded(Scene current)
    {
        PlayerPrefs.SetInt("score", currentScore);
        Debug.Log($"💾 スコア保存: {currentScore}");
    }

    private void OnDestroy()
    {
        // 念のためスコアを保存
        PlayerPrefs.SetInt("score", currentScore);
    }

    private void OnDisable()
    {
        // 念のためスコアを保存
        PlayerPrefs.SetInt("score", currentScore);
    }
}
