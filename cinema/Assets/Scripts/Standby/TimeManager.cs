using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

/* スタンバイシーンの時間管理プログラム

定義場所：スタンバイシーン
内容：

    jsonファイルからスケジュールを取得
    TitleDataInitializer.csで定義した曜日、シーン番号を取得
    取得した曜日のシーン番号を検索し、その10分前の時間からタイマーを開始する【UI表示も行う】

*/

// 映画の1件分の情報を表すクラス（インデックス、タイトル、時間、開始時間）
[System.Serializable]
public class MovieEntry
{
    public int index;
    public string title;
    public string duration;
    public string start;
}

// 各曜日ごとの映画スケジュールを格納するクラス
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
    public TextAsset jsonFile; // スケジュール情報が入ったJSONファイル

    [Header("UI")]
    public TMP_Text timeText; // 現在時刻を表示するテキストUI

    [Header("設定")]
    public string weekday = "月"; // 対象の曜日（例：月）
    public int index = 1;     // 対象のインデックス（例：1）
    public int people;


    [Header("進行スピード（秒で開始時間になる）")]

    // ☑️太田「この秒数がリアルタイムの準備時間になる」
    [SerializeField] private float realSecondsUntilStart = 20f; // 何秒かけて開始時間まで進めるか（実時間）
    

    private ScheduleData schedule;       // JSONから読み込まれる全スケジュール
    private DateTime initialTime;        // 映画開始時間の10分前（シミュレーション開始時刻）
    private DateTime startTime;          // 映画の開始時間
    private float elapsedRealTime = 0f;  // 経過した実時間（秒）
    private bool isTimeAdvancing = false; // 時間を進める処理を開始するかどうか

    void Start()
    {
        // PlayerPrefsに保存されたデータを読み込む
        int weekday = PlayerPrefs.GetInt("weekday", -1);  // デフォルト値を-1（データが無ければ）
        int index = PlayerPrefs.GetInt("index", -1);      // デフォルト値を-1
        int people = PlayerPrefs.GetInt("people", 0);       // デフォルト値を0
        
        LoadSchedule();      // JSONファイルを読み込む
        SetupInitialTime();  // 初期時刻を設定する
    }

    void Update()
    {
        if (!isTimeAdvancing) return; // 時間を進めるかどうかのチェック

        elapsedRealTime += Time.deltaTime; // 実時間を加算

        // 1秒あたりに進める仮想時間（分）＝10分を指定秒数で進める
        float simulatedMinutesPerRealSecond = 10f / realSecondsUntilStart;

        // 経過した実時間を元に仮想時間を進める
        DateTime currentTime = initialTime.AddMinutes(elapsedRealTime * simulatedMinutesPerRealSecond);
        timeText.text = currentTime.ToString("HH:mm"); // HH:mm形式で表示
    }

    // JSONファイルを読み込み、スケジュールに変換する
    void LoadSchedule()
    {
        if (jsonFile == null)
        {
            //Debug.LogError("JSONファイルが指定されていません。");
            return;
        }

        // UnityのJsonUtilityで読み込めるように、ラッパー形式にする
        string wrappedJson = "{\"data\":" + jsonFile.text + "}";
        ScheduleWrapper wrapper = JsonUtility.FromJson<ScheduleWrapper>(wrappedJson);
        schedule = wrapper.data;
    }

    // 対象の映画開始時間を読み取り、そこから10分前を初期時刻に設定する
    // ☑️太田「仮設定で10分前に設定してる」
    void SetupInitialTime()
    {
        if (schedule == null) return;

        List<MovieEntry> todaySchedule = GetDaySchedule(weekday); // 曜日ごとのスケジュールを取得
        if (todaySchedule == null) return;

        // 指定されたインデックスの映画を取得
        MovieEntry movie = todaySchedule.Find(m => m.index == index);
        if (movie != null && DateTime.TryParse(movie.start, out DateTime parsedStartTime))
        {
            startTime = parsedStartTime;                          // 開始時刻
            initialTime = parsedStartTime.AddMinutes(-10);        // 10分前を初期時刻とする
            elapsedRealTime = 0f;
            isTimeAdvancing = true;                               // 時間の進行を開始
        }
        else
        {
            timeText.text = "--:--"; // 開始時刻が取得できなかった場合
        }
    }

    // 曜日文字列からその日のスケジュールを取得する
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

    // JSONをUnityのJsonUtilityで読み込むためのラッパークラス
    [System.Serializable]
    private class ScheduleWrapper
    {
        public ScheduleData data;
    }
}
