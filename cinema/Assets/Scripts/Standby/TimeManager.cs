using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

/// <summary>
/// スタンバイシーンの時間管理プログラム
/// </summary>
[System.Serializable]
public class MovieEntry
{
    public int index;
    public string title;
    public string duration;
    public string start;
}

[System.Serializable]
public class ScheduleData
{
    public List<MovieEntry> 月;
    public List<MovieEntry> 火;
    public List<MovieEntry> 水;
    public List<MovieEntry> 木;
    public List<MovieEntry> 金;
}

public class TimeManager : MonoBehaviour
{
    [Header("JSONデータ")]
    public TextAsset jsonFile;

    [Header("UI")]
    public TMP_Text timeText;

    private int weekday;  // 0〜4（月〜金）
    private int index;
    private int people;
    private string weekdayStr;  // ← 追加：文字列の曜日（月〜金）

    [Header("進行スピード（秒で開始時間になる）")]
    [SerializeField] private float realSecondsUntilStart = 20f;

    private ScheduleData schedule;
    private DateTime initialTime;
    private DateTime startTime;
    private float elapsedRealTime = 0f;
    private bool isTimeAdvancing = false;

    void Start()
    {
        // プレイヤー設定取得
        weekday = PlayerPrefs.GetInt("weekday", -1);
        index = PlayerPrefs.GetInt("index", -1);
        people = PlayerPrefs.GetInt("people", 0);

        // int → string に変換（月〜金）
        weekdayStr = weekday switch
        {
            0 => "月",
            1 => "火",
            2 => "水",
            3 => "木",
            4 => "金",
            _ => null
        };

        LoadSchedule();
        SetupInitialTime();
    }

    void Update()
    {
        if (!isTimeAdvancing) return;

        elapsedRealTime += Time.deltaTime;

        float simulatedMinutesPerRealSecond = 10f / realSecondsUntilStart;

        DateTime currentTime = initialTime.AddMinutes(elapsedRealTime * simulatedMinutesPerRealSecond);

        if (timeText != null)
            timeText.text = currentTime.ToString("HH:mm");
    }

    void LoadSchedule()
    {
        if (jsonFile == null || string.IsNullOrWhiteSpace(jsonFile.text))
        {
            Debug.LogWarning("JSONファイルが未指定または空です。");
            return;
        }

        string wrappedJson = "{\"data\":" + jsonFile.text + "}";
        ScheduleWrapper wrapper = JsonUtility.FromJson<ScheduleWrapper>(wrappedJson);
        schedule = wrapper.data;
    }

    void SetupInitialTime()
    {
        if (schedule == null || string.IsNullOrEmpty(weekdayStr)) return;

        List<MovieEntry> todaySchedule = GetDaySchedule(weekdayStr);
        if (todaySchedule == null) return;

        MovieEntry movie = todaySchedule.Find(m => m.index == index);
        if (movie != null && DateTime.TryParse(movie.start, out DateTime parsedStartTime))
        {
            startTime = parsedStartTime;
            initialTime = parsedStartTime.AddMinutes(-10);
            elapsedRealTime = 0f;
            isTimeAdvancing = true;
        }
        else
        {
            if (timeText != null)
                timeText.text = "--:--";
        }
    }

    List<MovieEntry> GetDaySchedule(string weekday)
    {
        return weekday switch
        {
            "月" => schedule.月,
            "火" => schedule.火,
            "水" => schedule.水,
            "木" => schedule.木,
            "金" => schedule.金,
            _ => null,
        };
    }

    [System.Serializable]
    private class ScheduleWrapper
    {
        public ScheduleData data;
    }
}
