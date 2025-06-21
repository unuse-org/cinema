using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class VideoPlayerController : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private VideoClip[] videoClips = new VideoClip[5]; // 動画を5本設定

    private int index;
    private int movieIndex;

    void Start()
    {
        // PlayerPrefsからmovie番号（0〜4）を取得（初期値0）
        movieIndex = PlayerPrefs.GetInt("movie");
        //Debug.Log("movieIndex"+movieIndex);

        index = PlayerPrefs.GetInt("index");
        //Debug.Log("index = " +index);

        // clipIndex を 0 〜 4 の範囲に制限（0 から videoClips.Length - 1 まで）
        int clipIndex = Mathf.Clamp(movieIndex, 0, videoClips.Length - 1);

        if (videoClips[clipIndex] != null)
        {
            videoPlayer.clip = videoClips[clipIndex];
            videoPlayer.prepareCompleted += OnVideoPrepared;
            videoPlayer.loopPointReached += OnVideoFinished;
            videoPlayer.Prepare(); // 動画準備
        }
        else
        {
            Debug.LogWarning("VideoClip が設定されていません！");
        }
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        //再生処理
        rawImage.texture = videoPlayer.texture;
        videoPlayer.Play();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        //再生終了後、シーン移動処理
        //Debug.Log("動画の再生が終了しました（movie: " + PlayerPrefs.GetInt("movie", 1) + "）");
        if (index == 6)
        {
            SceneManager.LoadScene("End");
        }
        else
        {
            SceneManager.LoadScene("standby");
        }

    }
}
