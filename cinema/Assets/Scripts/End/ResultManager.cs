using UnityEngine;
using TMPro;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultScoreText;
    
    // RankingManagerへの参照を追加
    [SerializeField] private RankingManager rankingManager;

    void Start()
    {
        // PlayerPrefsから今回のスコアを読み込む
        int finalScore = PlayerPrefs.GetInt("score", 0);
        
        // UIに最終スコアを表示
        resultScoreText.text = "最終スコア: " + finalScore.ToString();

        // --- ここから追加 ---
        // RankingManagerが見つかれば、スコアを保存する
        if (rankingManager != null)
        {
            rankingManager.SaveScore(finalScore);
        }
        else
        {
            Debug.LogError("RankingManagerが設定されていません！");
        }
        // --- ここまで追加 ---
        
        // 一度使ったキーは削除しておく
        PlayerPrefs.DeleteKey("score");
    }
}