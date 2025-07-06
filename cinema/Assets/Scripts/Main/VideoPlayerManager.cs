using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

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
    private int score;

    private float accidentSpeed;
    private int speed;

    [Header("アクシデント制御")]
    [Tooltip("1本の動画で発生する最小アクシデント回数")]
    [SerializeField] private int minAccidentCount = 1;

    [Tooltip("1本の動画で発生する最大アクシデント回数")]
    [SerializeField] private int maxAccidentCount = 2;

    [SerializeField] private SurpriseHumansManager surpriseManager;

    private int accidentTargetCount = 0;
    private int accidentCount = 0;
    private double nextAccidentTime = -1;
    private bool accidentActive = false;
    private BubbleSpawner bubbleSpawner;

    public udp_receiver_speed receiver_speed;

    void Awake()
    {
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
        score = PlayerPrefs.GetInt("score", 0);

        Debug.Log($"Loaded movieIndex: {movieIndex}, sceneIndex: {sceneIndex}");

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

        accidentTargetCount = Random.Range(minAccidentCount, maxAccidentCount + 1);
        accidentCount = 0;
        ScheduleNextAccident();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("動画の再生が終了しました（movie: " + movieIndex + "）");
        if (sceneIndex == 5)
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

        if (!accidentActive && nextAccidentTime >= 0 &&
            videoPlayer.isPrepared && videoPlayer.time >= nextAccidentTime)
        {
            TriggerAccident();
            accidentCount++;
            ScheduleNextAccident();
        }

        if (accidentActive && receiver_speed != null)
        {
            int sensor = receiver_speed.Speed;
            bool shouldRelease =
                (accidentSpeed > 1f && sensor == 3) ||
                (accidentSpeed < 1f && sensor == 1);

            if (shouldRelease) ReleaseAccident();
        }
    }

    private void TriggerAccident()
    {
        accidentActive = true;

        // ✅ 修正箇所：ローカル変数にせず、フィールドに代入
        accidentSpeed = Random.value < 0.5f ? 0.5f : 1.5f;
        string accidentType = accidentSpeed > 1f ? "SpeedUp" : "SpeedDown";

        Debug.Log($"⚠️ アクシデント発生！ {(accidentType == "SpeedUp" ? "🚀【速度UP】" : "🐢【速度DOWN】")} x{accidentSpeed}");
        SetPlaybackSpeed(accidentSpeed);

        if (accidentSpeed > 1)
        {
            BubbleSpawner.Instance.SpawnBubbles(BubbleSpawner.Situation.SpeedUp);
        }
        else
        {
            BubbleSpawner.Instance.SpawnBubbles(BubbleSpawner.Situation.SpeedDown);
        }
    }

    private void ReleaseAccident()
    {
        Debug.Log("🎮 センサー入力（デバッグ）：ユーザーが対処行動を実行");

        accidentActive = false;
        SetPlaybackSpeed(1f);
        Debug.Log("✅ アクシデント解除：速度を通常（x1.0）に戻しました");

        score += 3;
        PlayerPrefs.SetInt("score", score);

        surpriseManager.SurpriseAllHumans();
        ScheduleNextAccident();

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

    private void ScheduleNextAccident()
    {
        if (accidentCount >= accidentTargetCount)
        {
            nextAccidentTime = -1;
            return;
        }

        double remainingTime = videoPlayer.length - videoPlayer.time;
        if (remainingTime < 5.0) return;

        double offset = Random.Range(3f, (float)(remainingTime - 1f));
        nextAccidentTime = videoPlayer.time + offset;
        Debug.Log($"📆 次のアクシデント予定時刻: {nextAccidentTime:F2} 秒");
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
