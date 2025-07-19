using UnityEngine;
using System.IO;

public class ScoreLogger : MonoBehaviour
{
    private string fileName = "score_log.csv";

    public void SaveScore(int score)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);

        // 既存のCSVに「,スコア」を追記（初回はそのまま）
        if (!File.Exists(path) || new FileInfo(path).Length == 0)
        {
            File.WriteAllText(path, score.ToString());
        }
        else
        {
            File.AppendAllText(path, "," + score.ToString());
        }

        Debug.Log($"✅ スコアを保存: {score} → {path}");
    }
}
