using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;


public class VideoPlayerManager : MonoBehaviour
{
    [Header("必須参照")]
    [Tooltip("動画を表示するRawImageをここに設定してください。")]
    [SerializeField] private RawImage rawImage;

    private VideoPlayer videoPlayer;

    [Tooltip("再生する動画クリップをここに設定してください（最大5本）。")]
    [SerializeField] private VideoClip[] videoClips = new VideoClip[5];

    [Tooltip("再生速度を表示するUIテキスト (TextMeshPro) を設定してください。")]
    [SerializeField] private TextMeshProUGUI speedDisplayText;

    [Tooltip("再生時間を表示するUIテキスト (TextMeshPro) を設定してください。")]
    [SerializeField] private TextMeshProUGUI videoTimeText;

    private int movieIndex;
    private int sceneIndex;
    private int people;

    private float accidentSpeed;
    private int speed;

    //[Header("アクシデント制御")]
    //[Tooltip("1本の動画で発生する最小アクシデント回数")]
    //[SerializeField] private int minAccidentCount = 1;

    // [Tooltip("1本の動画で発生する最大アクシデント回数")]
    // [SerializeField] private int maxAccidentCount = 2;

    [SerializeField] private SurpriseHumansManager surpriseManager;

    private int accidentTargetCount = 0;
    private int accidentCount = 0;
    private double nextAccidentTime = -1;
    private bool accidentActive = false;
    private BubbleSpawner bubbleSpawner;

    private udp_receiver_speed receiver_speed;

    [SerializeField] SceneBObjectStateManager  sceneBObjectStateManager; 

    private int  sensor = 0; 
    private Queue<double> accidentScheduleQueue = new Queue<double>();

    private bool allAccidentsScheduled = false;  // 全てのアクシデントがスケジュール済みかどうかを管理



    void Awake()
    {
        GameObject imuObject = GameObject.Find("M5_IMU_Speed_Wifi");
        if (imuObject != null)
        {
            receiver_speed = imuObject.GetComponent<udp_receiver_speed>();
            if (receiver_speed == null)
            {
                Debug.LogError("udp_receiver_speed スクリプトが M5_IMU_Speed_Wifi にアタッチされていません。");
            }
        }
        else
        {
            Debug.LogError("M5_IMU_Speed_Wifi オブジェクトが見つかりません。");
        }

        bubbleSpawner = FindObjectOfType<BubbleSpawner>();
        if (bubbleSpawner == null)
        {
            Debug.LogError("❌ BubbleSpawner が見つかりません");
        }

        videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayerManagerにVideoPlayerコンポーネントが見つかりません。");
            return;
        }

        if (rawImage == null)
        {
            Debug.LogError("RawImageが設定されていません。Inspectorで設定してください。");
            return;
        }

        if (speedDisplayText == null)
        {
            Debug.LogWarning("Speed Display Text (TMP) が設定されていません。再生速度のUI表示は行われません。");
        }

        movieIndex = PlayerPrefs.GetInt("movie", 0);
        sceneIndex = PlayerPrefs.GetInt("index", 0);
        people = PlayerPrefs.GetInt("people", 0);

        Debug.Log("sceneIndex: "+sceneIndex);

        int clipToPlayIndex = Mathf.Clamp(movieIndex, 0, videoClips.Length - 1);

        if (videoClips[clipToPlayIndex] != null)
        {
            videoPlayer.clip = videoClips[clipToPlayIndex];

            RenderTexture renderTexture = new RenderTexture((int)videoPlayer.clip.width, (int)videoPlayer.clip.height, 24);
            videoPlayer.targetTexture = renderTexture;
            rawImage.texture = renderTexture;

            videoPlayer.prepareCompleted += OnVideoPrepared;
            videoPlayer.loopPointReached += OnVideoFinished;
            videoPlayer.Prepare();
        }
        else
        {
            Debug.LogWarning($"VideoClip at index {clipToPlayIndex} は設定されていません！ movieIndex: {movieIndex}");
        }

        SetPlaybackSpeed(1f);
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        videoPlayer.Play();

        if (!allAccidentsScheduled)
        {
            accidentTargetCount = Mathf.RoundToInt(Mathf.Lerp(2, 20, Mathf.Clamp01(people / 35f)));
            Debug.Log($"🎯 アクシデント発生目標回数: {accidentTargetCount} （観客: {people}人）");

            accidentCount = 0;
            ScheduleAllAccidents();  // アクシデントをスケジュール

            allAccidentsScheduled = true;  // アクシデントのスケジュールが完了したことを記録
        }
    }




    void OnVideoFinished(VideoPlayer vp)
    {
        //Debug.Log("動画の再生が終了しました（movie: " + movieIndex + "）");
        if (sceneIndex == 6)
        {
            SceneManager.LoadScene("End");
        }
        else
        {
            SceneManager.LoadScene("standby");
        }
    }

    void Update()
    {
        UpdateVideoTimeDisplay();

        if (!accidentActive && accidentScheduleQueue.Count > 0 &&
            videoPlayer.isPrepared && videoPlayer.time >= accidentScheduleQueue.Peek())
        {
            accidentScheduleQueue.Dequeue();
            TriggerAccident();
            accidentCount++;
        }

        if (accidentActive && receiver_speed != null)
        {
            //センサー処理
            int sensor = receiver_speed.Speed;


            //デバッグ用キー入力処理
            // if (Input.GetKeyDown(KeyCode.LeftArrow))
            // {
            //     sensor = 1;
            //     //Debug.Log("← 左矢印キー入力 → sensor = 1");
            // }
            // else if (Input.GetKeyDown(KeyCode.RightArrow))
            // {
            //     sensor = 3;
            //     //Debug.Log("→ 右矢印キー入力 → sensor = 3");
            // }

            bool shouldRelease =
                (accidentSpeed > 1f && sensor == 1) ||
                (accidentSpeed < 1f && sensor == 3);

            if (shouldRelease) ReleaseAccident();
            sensor = 2;
        }
    }


    private void TriggerAccident()
    {
        accidentActive = true;

        // ✅ 修正箇所：ローカル変数にせず、フィールドに代入
        accidentSpeed = Random.value < 0.75f ? 0.5f : 1.2f;
        string accidentType = accidentSpeed > 1f ? "SpeedUp" : "SpeedDown";

        Debug.Log($"⚠️ アクシデント発生！ {(accidentType == "SpeedUp" ? "🚀【速度UP】" : "🐢【速度DOWN】")} x{accidentSpeed}");
        SetPlaybackSpeed(accidentSpeed);

        if (accidentSpeed > 1)
        {
            BubbleSpawner.Instance.SpawnBubbles(BubbleSpawner.Situation.SpeedDown);
        }
        else
        {
            BubbleSpawner.Instance.SpawnBubbles(BubbleSpawner.Situation.SpeedUp);
        }
    }

    private void ReleaseAccident()
    {
        //Debug.Log("🎮 センサー入力（デバッグ）：ユーザーが対処行動を実行");

        accidentActive = false;
        SetPlaybackSpeed(1f);
        Debug.Log("✅ アクシデント解除：速度を通常（x1.0）に戻しました");

        people += 1;
        PlayerPrefs.SetInt("people", people);
        sceneBObjectStateManager.ApplyDiffBasedActivation();

        surpriseManager.SurpriseAllHumans();

        // 既にアクシデントスケジュールが完了していれば再スケジュールは行わない
        if (!allAccidentsScheduled)
        {
            ScheduleAllAccidents();
        }

        // ✅ BubbleImage という名前の GameObject をすべて探して削除
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "BubbleImage")
            {
                Destroy(obj);
            }
        }
    }



    //private bool allAccidentsScheduled = false;  // アクシデントのスケジュールが一度だけ実行されるかを管理するフラグ

    private void ScheduleAllAccidents()
    {
        if (allAccidentsScheduled) return;  // すでにアクシデントがスケジュールされている場合は何もしない

        accidentScheduleQueue.Clear();

        double duration = videoPlayer.length;
        if (duration < 5.0) return;

        double start = 3.0;
        double end = duration - 3.0;

        // accidentTargetCount の値が大きすぎないように制御
        int targetAccidentCount = Mathf.Clamp(accidentTargetCount, 2, 20); // 目標回数を2から20の範囲で調整

        double interval = (end - start) / targetAccidentCount;

        for (int i = 0; i < targetAccidentCount; i++)
        {
            double randomOffset = Random.Range(-1f, 1f);
            double time = start + interval * i + randomOffset;
            time = Mathf.Clamp((float)time, (float)start, (float)end);
            accidentScheduleQueue.Enqueue(time);
        }

        Debug.Log($"📆 {targetAccidentCount}個のアクシデントをスケジュール済み");

        allAccidentsScheduled = true;  // スケジュール完了フラグを立てる
    }

    private void SetPlaybackSpeed(float speed)
    {
        if (videoPlayer != null) videoPlayer.playbackSpeed = speed;

        if (speedDisplayText != null)
            speedDisplayText.text = $"x{speed:0.0}";
    }

    public double GetVideoCurrentTime()
    {
        if (videoPlayer != null && videoPlayer.isPrepared)
        {
            return videoPlayer.time;
        }
        return 0.0;
    }

    private void UpdateVideoTimeDisplay()
    {
        if (videoPlayer != null && videoPlayer.isPrepared && videoTimeText != null)
        {
            double time = videoPlayer.time;
            int minutes = (int)(time / 60);
            int seconds = (int)(time % 60);
            videoTimeText.text = $"{minutes:D2}:{seconds:D2}";
        }
    }
}
