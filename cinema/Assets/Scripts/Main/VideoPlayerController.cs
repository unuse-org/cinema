using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoPlayerController : MonoBehaviour
{
    public RawImage rawImage;
    public VideoPlayer videoPlayer;
    public VideoClip[] videoClips = new VideoClip[5]; // インスペクターで5本まで設定可能

    void Start()
    {
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnVideoPrepared;
    }

    void Update()
    {
        // キー入力を検出して動画を切り替える
        if (Input.GetKeyDown(KeyCode.Alpha1)) PlayVideo(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) PlayVideo(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) PlayVideo(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) PlayVideo(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) PlayVideo(4);
    }

    void PlayVideo(int index)
    {
        if (index >= 0 && index < videoClips.Length && videoClips[index] != null)
        {
            videoPlayer.Stop(); // 現在の動画を止める
            videoPlayer.clip = videoClips[index];
            videoPlayer.Prepare(); // 動画を準備（OnVideoPreparedが呼ばれる）
        }
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        rawImage.texture = videoPlayer.texture;
        videoPlayer.Play();
    }
}
