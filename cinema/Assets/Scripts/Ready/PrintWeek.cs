using UnityEngine;
using TMPro;


public class PrintWeek : MonoBehaviour
{
    private int weekday;
    public TMP_Text text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        weekday = PlayerPrefs.GetInt("weekday", weekday);
        
        switch (weekday)
        {
            case 1: text.text = "月曜日"; break;
            case 2: text.text = "火曜日"; break;
            case 3: text.text = "水曜日"; break;
            case 4: text.text = "木曜日"; break;
            case 5: text.text = "金曜日"; break;
            default: text.text = "不明な曜日"; break;
        }
        // 最初は完全に表示される状態に
        text.alpha = 1f;

        // フェードアウト開始
        StartCoroutine(FadeOutText(3f));

    }

    private System.Collections.IEnumerator FadeOutText(float duration)
    {
        float elapsed = 0f;
        float startAlpha = text.alpha;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            text.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
            yield return null;
        }

        text.alpha = 0f; // 念のため完全に透明に
    }

}
