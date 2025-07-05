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

    private int currentSpeedIndex = 1; // 現在の通常再生速度（等倍）

    private int movieIndex;
    private int sceneIndex;
    private int score;

    [Header("アクシデント制御")]
    [Tooltip("1本の動画で発生する最小アクシデント回数")]
    [SerializeField] private int minAccidentCount = 1;

    [Tooltip("1本の動画で発生する最大アクシデント回数")]
    [SerializeField] private int maxAccidentCount = 2;

    private int accidentTargetCount = 0;
    private int accidentCount = 0;
    private double nextAccidentTime = -1;
    private bool accidentActive = false;

    void Awake()
    {
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

        // アクシデント回数を決定（1～2回）
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
        // センサー入力再現（矢印キー）＋アクシデント解除
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (accidentActive)
            {
                Debug.Log("🎮 センサー入力（デバッグ）：ユーザーが対処行動を実行");

                accidentActive = false;
                SetPlaybackSpeed(1f);
                Debug.Log("✅ アクシデント解除：速度を通常（x1.0）に戻しました");
                ScheduleNextAccident();
            }
            else
            {
                // アクシデントが発生していないのに入力された場合
                Debug.Log("⚠️ センサー入力がありましたが、アクシデントは発生していません。無視します。");
            }
        }

        // アクシデント発生チェック
        if (!accidentActive && nextAccidentTime > 0 && videoPlayer.time >= nextAccidentTime)
        {
            TriggerAccident();
            accidentCount++;
        }

        // 再生時間の表示更新
        UpdateVideoTimeDisplay();
    }


    private void TriggerAccident()
    {
        accidentActive = true;

        float accidentSpeed = Random.value < 0.5f ? 0.5f : 1.5f;
        string accidentType = accidentSpeed > 1f ? "SpeedUp" : "SpeedDown";

        Debug.Log($"⚠️ アクシデント発生！ {(accidentType == "SpeedUp" ? "🚀【速度UP】" : "🐢【速度DOWN】")} x{accidentSpeed}");
        SetPlaybackSpeed(accidentSpeed);

        // 吹き出しを生成（正しく accidentType を渡す！）
        BubbleSpawner.Instance.SpawnBubbles(accidentType);
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
