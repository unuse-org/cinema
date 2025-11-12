using System;
using System.Collections.Generic;

[Serializable]
public class ScoreData
{
    public int scores;

    public ScoreData(int scores)
    {
        this.scores = scores;
    }
}

[Serializable]
public class Ranking
{
    public List<ScoreData> ranking = new List<ScoreData>();

    // コンストラクタで初期化
    public Ranking()
    {
        for (int i = 0; i < 5; i++)
        {
            ranking.Add(new ScoreData(0));
        }
    }
}