using UnityEngine;               // ← これが必要！
using UnityEngine.Video;         // ← これが必要！
using UnityEngine.UI;            // ← RawImageはこの中にある

public class StartVideo : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public RawImage rawImage;
    public VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnVideoPrepared;
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        rawImage.texture = videoPlayer.texture;
        videoPlayer.Play();
    }
}
