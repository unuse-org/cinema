using System;
using System.Collections.Generic;

[Serializable]
public class ScoreData
{
    public int score;

    public ScoreData(int score)
    {
        this.score = score;
    }
}

[Serializable]
public class Ranking
{
    public List<ScoreData> ranking = new List<ScoreData>();
}