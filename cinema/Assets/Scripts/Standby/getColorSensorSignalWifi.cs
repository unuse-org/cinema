using System.Collections.Generic;
using UnityEngine;

/* カラーセンサー処理プログラム（ログ強化版）

   - 色と対応した映画名を配列で定義
   - JSON から曜日別の上映スケジュールをロード
   - M5Stack から色信号を受信し lastSignal に保持
   - CheckSchedule() で JSON のタイトルと現在検知しているタイトルを比較
*/

public class GetColorSensorSignalWifi : MonoBehaviour
{
    [Header("▼ Inspector 設定")]
    [SerializeField] private TextAsset jsonFile;
    [SerializeField] private GameObject M5_Color;

    // ───────── 内部変数 ─────────
    private UdpReceiver colorSensor;

    private readonly string[] movieTitles =
        { "フクロウ仮面", "チョコミント", "こえかけさん", "クリムゾン", "鯨の声", "サメ遊戯" };
    
    private int lastSignal = -1;   // センサー取得値 (0‑5)
    private int index     = -1;    // 呼び出し側で設定される上映インデックス
    private int weekday   = -1;    // PlayerPrefs から取得 (0=月 … 4=金)

    private UdpReceiver receiver;

    // ───────── JSON 用クラス ─────────
    [System.Serializable] public class ScheduleItem { public int index; public string title; public string duration; public string start; }
    [System.Serializable] public class DaySchedule
    {
        public List<ScheduleItem> 月, 火, 水, 木, 金;
    }
    private DaySchedule scheduleData;

    void Awake()
    {
        GameObject imuObject = GameObject.Find("M5_Color_Wifi");
        if (imuObject != null)
        {
            receiver = imuObject.GetComponent<UdpReceiver>();
            if (receiver == null)
            {
                Debug.LogError("M5_Color_Wifi スクリプトが M5_IMU_Speed_Wifi にアタッチされていません。");
            }
        }
    }

    //============================================================
    //                           Start
    //============================================================
    void Start()
    {
        // 1) 曜日読み込み（0〜4）
        weekday = PlayerPrefs.GetInt("weekday", -1);
        //Debug.Log($"[Start]  weekday (PlayerPrefs) = {weekday}"); // 例：4 = 金曜日

        // 1.5) index読み込み（各曜日の上映順）
        index = PlayerPrefs.GetInt("index", -1);
        //Debug.Log($"[Start]  index (PlayerPrefs) = {index}");

        // 2) JSON 読み込み
        if (jsonFile == null)
        {
            //Debug.LogError("[Start]  jsonFile が設定されていません！");
        }
        else
        {
            scheduleData = JsonUtility.FromJson<DaySchedule>(jsonFile.text);
            // Debug.Log($"[Start]  JSON 読込結果: " +
            //         $"月={scheduleData?.月?.Count ?? 0}, 火={scheduleData?.火?.Count ?? 0}, " +
            //         $"水={scheduleData?.水?.Count ?? 0}, 木={scheduleData?.木?.Count ?? 0}, 金={scheduleData?.金?.Count ?? 0}");
        }


        // 3) カラーセンサー取得
        if (M5_Color == null)
        {
            //Debug.LogError("[Start]  M5_Color オブジェクトが設定されていません！");
        }
        else
        {
            colorSensor = M5_Color.GetComponent<UdpReceiver>();
            if (colorSensor == null)
                Debug.LogError("[Start]  UdpReceiver コンポーネントが見つかりません (M5_Color)");
        }
    }

    //============================================================
    //                 UpdateSensorSignal ― センサー取得
    //============================================================
    public void UpdateSensorSignal()
    {
        if (receiver == null) { Debug.LogWarning("[Signal] colorSensor が null"); return; }
        lastSignal = receiver.color;

        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     lastSignal = 0;
        //     Debug.Log("A pressed → lastSignal = 1");
        // }
        // else if (Input.GetKeyDown(KeyCode.S))
        // {
        //     lastSignal = 1;
        //     Debug.Log("S pressed → lastSignal = 2");
        // }
        // else if (Input.GetKeyDown(KeyCode.D))
        // {
        //     lastSignal = 2;
        //     Debug.Log("D pressed → lastSignal = 3");
        // }
        // else if (Input.GetKeyDown(KeyCode.F))
        // {
        //     lastSignal = 3;
        //     Debug.Log("F pressed → lastSignal = 4");
        // }
        // else if (Input.GetKeyDown(KeyCode.G))
        // {
        //     lastSignal = 4;
        //     Debug.Log("G pressed → lastSignal = 5");
        // }
        // else if (Input.GetKeyDown(KeyCode.H))
        // {
        //     lastSignal = 5;
        //     Debug.Log("H pressed → lastSignal = 6");
        // }
    }

    //============================================================
    //                   CheckSchedule ― タイトル照合
    //============================================================
    public bool CheckSchedule()
    {
        //Debug.Log($"[Check]   参数 ==> weekday={weekday}, index={index}, lastSignal={lastSignal}");

       List<ScheduleItem> selectedDay = weekday switch
        {
            0 => scheduleData?.月,
            1 => scheduleData?.火,
            2 => scheduleData?.水,
            3 => scheduleData?.木,
            4 => scheduleData?.金,
            _ => null
        };

        if (selectedDay == null)
        {
            //Debug.LogWarning($"[Check]   曜日が無効 ({weekday}) もしくは JSON に該当リストなし");
            return false;
        }

        // 曜日名の表示用
        string[] weekdayNames = { "月", "火", "水", "木", "金" };
        string currentWeekdayName = (weekday >= 0 && weekday <= 4) ? weekdayNames[weekday] : "不明";

        Debug.Log($"[Check] ▶️ 現在チェック中：{currentWeekdayName}曜日 の index={index}");

        // 2) インデックス範囲チェック
        if (index < 0 || index >= selectedDay.Count)
        {
            //Debug.LogWarning($"[Check]   index が範囲外: index={index}, selectedDay.Count={selectedDay.Count}");
            return false;
        }

        // 3) センサー値範囲チェック
        if (lastSignal < 0 || lastSignal >= movieTitles.Length)
        {
            //Debug.LogWarning($"[Check]   lastSignal が範囲外: {lastSignal}");
            return false;
        }

        // 4) タイトル比較
        string jsonTitle     = selectedDay[index].title;
        string currentTitle  = movieTitles[lastSignal];
        bool   isMatch       = jsonTitle == currentTitle;

        Debug.Log($"[Check]   比較: JSON=\"{jsonTitle}\"  vs  Sensor=\"{currentTitle}\"  →  {isMatch}");

        //成功失敗に関わらず保存
        PlayerPrefs.SetInt("movie", lastSignal);
        PlayerPrefs.Save();
        //Debug.Log($"[Check]   成功: movie 番号 {lastSignal} を保存済み");
        return isMatch;
    }

    //============================================================
    //                 外部から index を設定する補助関数
    //============================================================
    public void SetIndex(int idx)
    {
        index = idx;
        //Debug.Log($"[SetIdx]  index を {index} に設定");
    }
}
