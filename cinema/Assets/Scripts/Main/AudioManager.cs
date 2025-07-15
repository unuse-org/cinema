using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AudioSource audioSource;        // AudioSource を Inspector でセット
    public AudioClip cheerClip;            // 拍手や歓声などの音
    public AudioClip clapClip;

    public void PlayAudio()
    {
        if (audioSource != null && cheerClip != null)
        {
            audioSource.clip = cheerClip;
            audioSource.clip = clapClip;
            audioSource.loop = false;
            audioSource.Play();
        }
    }
    public void StopAudio()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void PlayCheer()
    {
        PlayClip(cheerClip);
    }

    public void PlayClap()
    {
        PlayClip(clapClip);
    }

    private void PlayClip(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioSource または AudioClip が設定されていません");
        }
    }
}
