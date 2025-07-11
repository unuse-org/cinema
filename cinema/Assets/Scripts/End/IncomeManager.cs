using UnityEngine;
using TMPro;
using System.Text; 

public class IncomeManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI IncomeText; 
    [SerializeField] private RankingManager rankingManager;
    private int totalScore = 0;

    void Start()
    {
        DisplayIncome();
    }

    private void DisplayIncome()
    {
        // RankingManagerからランキングデータを読み込む
        Ranking rankingData = rankingManager.LoadRanking();

        if (rankingData.ranking.Count == 0)
        {
            IncomeText.text = "まだスコアがありません";
            return;
        }

        // StringBuilderを使って効率的に文字列を結合する
        StringBuilder builder = new StringBuilder();

        for (int i = 0; i < rankingData.ranking.Count; i++)
        {
            totalScore += rankingData.ranking[i].score;
        }

        builder.AppendLine($"本日の興行収入 {totalScore} 点");

        // 最終的な文字列をテキストに設定
        IncomeText.text = builder.ToString();
    }
}
