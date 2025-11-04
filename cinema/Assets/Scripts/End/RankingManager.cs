using UnityEngine;
using System.Linq; 

public class RankingManager : MonoBehaviour
{
    private const string RankingKey = "rankingData"; // PlayerPrefsのキー
    private const int MaxRankingCount = 100; // ランキングの最大保存数

    // 新しいスコアをランキングに追加するメソッド
    public void SaveScore(int newScore)
    {
        // 1. PlayerPrefsから既存のランキングを読み込む
        Ranking ranking = LoadRanking();

        // 2. 新しいスコアを追加
        ranking.ranking.Add(new ScoreData(newScore));

        // 3. スコアの高い順に並べ替え（降順ソート）
        ranking.ranking = ranking.ranking.OrderByDescending(data => data.scores).ToList();

        // 4. 最大保存数を超えていたら、古いデータを削除
        if (ranking.ranking.Count > MaxRankingCount)
        {
            ranking.ranking.RemoveRange(MaxRankingCount, ranking.ranking.Count - MaxRankingCount);
        }

        // 5. JSON形式に変換してPlayerPrefsに保存
        string json = JsonUtility.ToJson(ranking);
        PlayerPrefs.SetString(RankingKey, json);
        PlayerPrefs.Save();
        
        //Debug.Log("Ranking saved!");
    }

    // 保存されているランキングを読み込むメソッド
    public Ranking LoadRanking()
    {
        string json = PlayerPrefs.GetString(RankingKey, "");
        if (string.IsNullOrEmpty(json))
        {
            return new Ranking(); // 保存データがなければ新しいRankingを返す
        }
        else
        {
            // JSONをRankingクラスにデシリアライズして返す
            return JsonUtility.FromJson<Ranking>(json);
        }
    }
}