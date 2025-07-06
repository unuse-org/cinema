using UnityEngine;
using TMPro;
using System.Text; 

public class RankingViewer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankingText; 
    [SerializeField] private RankingManager rankingManager;

    void Start()
    {
        DisplayRanking();
    }

    private void DisplayRanking()
    {
        // RankingManagerからランキングデータを読み込む
        Ranking rankingData = rankingManager.LoadRanking();

        if (rankingData.ranking.Count == 0)
        {
            rankingText.text = "まだスコアがありません";
            return;
        }

        // StringBuilderを使って効率的に文字列を結合する
        StringBuilder builder = new StringBuilder();

        for (int i = 0; i < rankingData.ranking.Count; i++)
        {
            int rank = i + 1;
            int score = rankingData.ranking[i].score;
            builder.AppendLine($"{rank}位: {score} 人");
        }

        // 最終的な文字列をテキストに設定
        rankingText.text = builder.ToString();
    }
}