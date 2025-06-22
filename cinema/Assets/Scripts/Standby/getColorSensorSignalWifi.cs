using System.Collections.Generic;
using UnityEngine;

/*カラーセンサー処理プログラム

定義場所：スタンバイシーン
内容：

    色と対応した映画名をリスト型で定義
    jsonファイルから、映画名を取得
    カラーセンサーから値を常時取得し、認識している色を保存
    GetSensorSignal.csから呼ばれた時、保存している色と対応する映画の色を比較し、比較内容を返す

*/
public class getColorSensorSignalWifi : MonoBehaviour
{
    [SerializeField] private TextAsset jsonFile;
    [SerializeField] private GameObject M5_Color;

    private UdpReceiver colorSensor;

    private string[] movieTitles = { "フクロウ仮面", "チョコミント" };
    private int lastSignal = -1;

    private int index;
    private int weekday;

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
        index = PlayerPrefs.GetInt("index");
        weekday = PlayerPrefs.GetInt("weekday");
        if (jsonFile != null)
        {
            scheduleData = JsonUtility.FromJson<DaySchedule>(jsonFile.text);
        }
        else
        {
            //Debug.LogError("jsonFile が設定されていません。");
        }
        if (M5_Color != null)
        {
            colorSensor = M5_Color.GetComponent<UdpReceiver>();
            if (colorSensor == null)
            {
                Debug.LogError("SR_Color コンポーネントが M5_Color オブジェクトに見つかりません.");
            }
        }
        else
        {
            Debug.LogError("M5_Color オブジェクトが設定されていません．");
        }
    }

    void Update()
    {
        // //
        // if (Input.GetKeyDown(KeyCode.Alpha6)) lastSignal = 0;
        // else if (Input.GetKeyDown(KeyCode.Alpha7)) lastSignal = 1;
        // else if (Input.GetKeyDown(KeyCode.Alpha8)) lastSignal = 2;
        // else if (Input.GetKeyDown(KeyCode.Alpha9)) lastSignal = 3;
        // else if (Input.GetKeyDown(KeyCode.Alpha0)) lastSignal = 4;
        if(colorSensor != null)
        {
            lastSignal = colorSensor.color; // カラーセンサーからの信号を取得
            Debug.Log("カラーセンサーからの信号: " + lastSignal);
        }
    }

    public bool CheckSchedule()
    {
        //index番号とweekdayの番号から、曜日とシーン番号を参照し、タイトルを取得
        List<ScheduleItem> selectedDay = null;
        switch (weekday)
        {
            case 0: selectedDay = scheduleData.月; break; // 月曜日
            case 1: selectedDay = scheduleData.火; break; // 火曜日
            case 2: selectedDay = scheduleData.水; break; // 水曜日
            case 3: selectedDay = scheduleData.木; break; // 木曜日
            case 4: selectedDay = scheduleData.金; break; // 金曜日
        }
        if (selectedDay != null && index >= 0 && index < selectedDay.Count)
        {
            string jsonTitle = selectedDay[index].title; // インデックスに対応するタイトルを取得
            //Debug.Log(jsonTitle);

            string currentTitle = movieTitles[lastSignal];

            Debug.Log($"比較: JSON = {jsonTitle}, 選択 = {currentTitle}");

            // メイン関数用に、映画番号を保存
            PlayerPrefs.SetInt("movie", lastSignal);
            Debug.Log("movieIndex000"+lastSignal);
            PlayerPrefs.Save();
            //Debug.Log(lastSignal);

            return jsonTitle == currentTitle;
        }
        else
        {
            Debug.LogWarning("無効な曜日またはインデックス");
            return false;
        }
    }
}
