using System.Collections.Generic;
using UnityEngine;

public class getcolorsensorsignal : MonoBehaviour
{
    [SerializeField] private TextAsset jsonFile;

    private string[] movieTitles = { "フクロウ仮面", "チョコミント" };
    private int lastSignal = -1;

    [System.Serializable]
    public class ScheduleItem
    {
        public int index;
        public string title;
        public string duration;
        public string start;
    }

    [System.Serializable]
    public class DaySchedule
    {
        public List<ScheduleItem> 月;
        public List<ScheduleItem> 火;
        public List<ScheduleItem> 水;
        public List<ScheduleItem> 木;
        public List<ScheduleItem> 金;
    }

    private DaySchedule scheduleData;

    void Start()
    {
        if (jsonFile != null)
        {
            scheduleData = JsonUtility.FromJson<DaySchedule>(jsonFile.text);
        }
        else
        {
            Debug.LogError("jsonFile が設定されていません。");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha6)) lastSignal = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha7)) lastSignal = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha8)) lastSignal = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha9)) lastSignal = 3;
        else if (Input.GetKeyDown(KeyCode.Alpha0)) lastSignal = 4;
    }

    public bool CheckSchedule()
    {
        if (scheduleData == null || scheduleData.月 == null || scheduleData.月.Count < 1)
        {
            Debug.LogError("スケジュールデータが正しく読み込まれていません。");
            return false;
        }

        if (lastSignal < 0 || lastSignal >= movieTitles.Length)
        {
            Debug.LogWarning("lastSignal の値が無効です: " + lastSignal);
            return false;
        }

        string jsonTitle = scheduleData.月[0].title; // 月曜日 index 1番目（0-based index）
        string currentTitle = movieTitles[lastSignal];

        Debug.Log($"比較: JSON = {jsonTitle}, 選択 = {currentTitle}");

        return jsonTitle == currentTitle;
    }
}
