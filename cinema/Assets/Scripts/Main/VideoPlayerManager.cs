using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 
using TMPro; // TextMeshProを使用するために必要

public class VideoPlayerManager : MonoBehaviour
{
    [Tooltip("動画を表示するRawImageをここに設定してください。")]
    [SerializeField] private RawImage rawImage;

    private VideoPlayer videoPlayer;

    [Tooltip("再生する動画クリップをここに設定してください（最大5本）。")]
    [SerializeField] private VideoClip[] videoClips = new VideoClip[5];

    // --- 変更点 ---
    [Tooltip("再生速度の各段階を設定してください。中央（等倍）を含め3段階です。")]
    [SerializeField] private float[] playbackSpeeds = { 0.5f, 1.0f, 2.0f }; // スロー(0.5x), 等倍(1.0x), 早送り(2.0x)
    [Tooltip("現在の再生速度を表示するUIテキスト (TextMeshPro) を設定してください。")]
    [SerializeField] private TextMeshProUGUI speedDisplayText;

    [Tooltip("再生時間を表示するUIテキスト (TextMeshPro) を設定してください。")]
    [SerializeField] private TextMeshProUGUI videoTimeText;

    private int currentSpeedIndex = 1; // 初期速度はplaybackSpeeds配列のインデックス1 (等倍) に設定
    // --- ここまで変更点 ---

    private int movieIndex;
    private int sceneIndex;

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
            Debug.LogWarning("Speed Display Text (TMP) が設定されていません。Inspectorで設定してください。再生速度のUI表示は行われません。");
        }

        movieIndex = PlayerPrefs.GetInt("movie", 0);
        sceneIndex = PlayerPrefs.GetInt("index", 0);

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

        // 初期速度を適用し、UIを更新
        SetPlaybackSpeed(playbackSpeeds[currentSpeedIndex]);
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        //Debug.Log("動画の準備が完了しました。再生を開始します。");
        videoPlayer.Play();
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
        /*太田☑️ここに左加速度センサーの処理を入れる
                下のプログラムはデバッグ用
        
        
        
        */
        // デバッグ用: 左右の矢印キーで再生速度を変更
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            AdjustSpeed(-1); // 速度を低下
            // MovieDataLoader を取得
            MovieDataLoader loader = FindObjectOfType<MovieDataLoader>();
            if (loader == null)
            {
                Debug.LogError("MovieDataLoader が見つかりません！");
                return;
            }
            // VideoPlayerController などから現在再生中の時間を取得
            double currentTime = loader.GetComponent<UnityEngine.Video.VideoPlayer>()?.time ?? 0.0;

            // 条件（ここでは例として true）
            bool inputCondition = true;

            bool result = loader.CheckMovieCondition((float)currentTime, inputCondition);

            Debug.Log($"🎬 判定結果: {(result ? "成功" : "失敗")}");
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            AdjustSpeed(1); // 速度を上昇
            // MovieDataLoader を取得
            MovieDataLoader loader = FindObjectOfType<MovieDataLoader>();
            if (loader == null)
            {
                Debug.LogError("MovieDataLoader が見つかりません！");
                return;
            }

            // VideoPlayerController などから現在再生中の時間を取得
            double currentTime = loader.GetComponent<UnityEngine.Video.VideoPlayer>()?.time ?? 0.0;

            // 条件（ここでは例として true）
            bool inputCondition = true;

            bool result = loader.CheckMovieCondition((float)currentTime, inputCondition);

            Debug.Log($"🎬 判定結果: {(result ? "成功" : "失敗")}");
        }
        // ✅ 再生時間表示処理
        UpdateVideoTimeDisplay();
    }

    /// <summary>
    /// 再生速度を調整します。
    /// </summary>
    /// <param name="direction">速度変更の方向 (-1で低下, 1で上昇)</param>
    private void AdjustSpeed(int direction)
    {
        currentSpeedIndex += direction;
        // 速度インデックスを配列の範囲内に制限
        currentSpeedIndex = Mathf.Clamp(currentSpeedIndex, 0, playbackSpeeds.Length - 1);

        SetPlaybackSpeed(playbackSpeeds[currentSpeedIndex]);
    }

    /// <summary>
    /// VideoPlayerの再生速度を設定し、UIを更新します。
    /// </summary>
    /// <param name="speed">設定する再生速度</param>
    public void SetPlaybackSpeed(float speed)
    {
        if (videoPlayer != null)
        {
            videoPlayer.playbackSpeed = speed;
            //Debug.Log($"再生速度が {speed}x に変更されました。");
            UpdateSpeedDisplay(speed);
        }
    }

    /// <summary>
    /// UIに現在の再生速度を表示します。
    /// </summary>
    /// <param name="speed">表示する速度</param>
    private void UpdateSpeedDisplay(float speed)
    {
        if (speedDisplayText != null)
        {
            speedDisplayText.text = $"{speed.ToString("F2")}x"; // 小数点以下2桁まで表示
        }
    }

    // 他のスクリプトから現在の再生時間を取得するためのメソッド
    public double GetVideoCurrentTime()
    {
        if (videoPlayer != null && videoPlayer.isPrepared)
        {
            return videoPlayer.time;
        }
        return 0.0;
    }
    /// <summary>
    /// 再生中の動画の現在時刻をUIに表示します。
    /// </summary>
    private void UpdateVideoTimeDisplay()
    {
        if (videoPlayer != null && videoPlayer.isPrepared && videoTimeText != null)
        {
            double time = videoPlayer.time;
            double duration = videoPlayer.length;

            // 時間を mm:ss 形式にする
            int minutes = (int)(time / 60);
            int seconds = (int)(time % 60);
            int totalMinutes = (int)(duration / 60);
            int totalSeconds = (int)(duration % 60);

            videoTimeText.text = $"{minutes:D2}:{seconds:D2}";
        }
    }
}