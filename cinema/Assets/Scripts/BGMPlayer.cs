using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BGMPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip bgmClip; // 再生するBGMをInspectorで設定

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // AudioSourceの初期設定
        audioSource.clip = bgmClip;
        audioSource.loop = true;         // ループ再生を有効にする
        audioSource.playOnAwake = false; // 自動再生をオフにして、明示的に再生する
        audioSource.Play();              // BGMを再生
    }
}
