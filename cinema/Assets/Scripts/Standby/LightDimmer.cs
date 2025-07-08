using UnityEngine;
using System.Collections;

public class LightDimmer : MonoBehaviour
{
    public GameObject lightsParent;
    public float dimDuration = 5f;
    public float minIntensity = 0f;
    private Light[] lights;
    private bool isDimming = false;
    [SerializeField] private AudioClip bgmClip; // Inspectorで設定できるBGM
    private AudioSource audioSource;
    private bool hasPlayed = false; // 一度だけ再生するフラグ

    void Start()
    {
        lights = lightsParent.GetComponentsInChildren<Light>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSourceがこのGameObjectにアタッチされていません。");
        }
    }

    public IEnumerator StartDimming()
    {
        PlayBGMOnce();

        if (isDimming) yield break;
        isDimming = true;

        float elapsedTime = 0f;
        float[] initialIntensities = new float[lights.Length];
        for (int i = 0; i < lights.Length; i++)
        {
            initialIntensities[i] = lights[i].intensity;
        }

        while (elapsedTime < dimDuration)
        {
            elapsedTime += Time.deltaTime;
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].intensity = Mathf.Lerp(initialIntensities[i], minIntensity, elapsedTime / dimDuration);
            }
            yield return null;
        }

        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].intensity = minIntensity;
        }

        isDimming = false;
    }
    // クラス内のメソッドとして BGM を一度だけ再生
    private void PlayBGMOnce()
    {
        if (hasPlayed || audioSource == null || bgmClip == null)
        {
            return;
        }

        audioSource.clip = bgmClip;
        audioSource.PlayDelayed(2.5f); // 再生開始までさらに2.5秒遅らせる
        hasPlayed = true;

        Debug.Log("BGM を一度だけ再生しました。");
    }
}
