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

        // 表示
        weekdayText.text = weekdays[weekdayIndex] + "曜日";
    }
}
