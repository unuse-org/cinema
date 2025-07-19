using UnityEngine;
using TMPro;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultScoreText;
    [SerializeField] private RankingManager rankingManager;

    void Start()
    {
        // PlayerPrefsから今回のスコアを読み込む
        int finalScore = PlayerPrefs.GetInt("Game_Score_Current", 0);

        // UIに最終スコアを表示
        resultScoreText.text = "あなたの得点: " + finalScore.ToString() + "円";

        // RankingManagerが見つかればスコアを保存する
        if (rankingManager != null)
        {
            rankingManager.SaveScore(finalScore);
        }
        else
        {
            Debug.LogError("RankingManagerが設定されていません！");
        }

        //次回必要な値を削除
        PlayerPrefs.DeleteKey("weekday");
        PlayerPrefs.DeleteKey("index");
        PlayerPrefs.DeleteKey("Game_Score_Current");
        PlayerPrefs.DeleteKey("people");

        Debug.Log("データ削除後" + PlayerPrefs.GetInt("Game_Score_Current"));
        PlayerPrefs.Save();
    }
}
