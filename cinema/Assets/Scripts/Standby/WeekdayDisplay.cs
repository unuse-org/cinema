using UnityEngine;
using TMPro;

public class WeekdayDisplay : MonoBehaviour
{
    public TextMeshProUGUI weekdayText;

    void Start()
    {
        // PlayerPrefsから weekday を取得（0=月, 1=火, ..., 4=金）
        int weekdayIndex = PlayerPrefs.GetInt("weekday");

        // 月〜金のみ対応（0〜4）
        if (weekdayIndex < 0 || weekdayIndex > 4)
        {
            Debug.LogWarning("曜日インデックスが無効です。PlayerPrefsの 'weekday' が 0〜4 になっているか確認してください。");
            weekdayText.text = "曜日不明";
            return;
        }

        // 月〜金の曜日リスト
        string[] weekdays = { "月", "火", "水", "木", "金" };

        // 曜日ごとの色（RGBで指定）
        Color[] weekdayColors = {
            new Color(0.6f, 0.6f, 0.6f), // 月（銀）: 少し暗めの銀色
            new Color(0.8f, 0.2f, 0.2f), // 火（赤）: 少し落ち着いた赤
            new Color(0.2f, 0.2f, 0.6f), // 水（青）: 少し落ち着いた青
            new Color(0.2f, 0.6f, 0.2f), // 木（緑）: 少し落ち着いた緑
            new Color(0.8f, 0.7f, 0.2f)  // 金（黄色）: 少し落ち着いた黄色
        };
        // 表示
        weekdayText.text = weekdays[weekdayIndex];
        // 曜日ごとの色を設定
        weekdayText.color = weekdayColors[weekdayIndex];
    }
}
