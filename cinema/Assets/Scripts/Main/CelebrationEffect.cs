using UnityEngine;

public class CelebrationEffect : MonoBehaviour
{
    public ParticleSystem confettiEffect;
    public AudioSource cheerAudio;

    public void PlayCelebration()
    {
        if (confettiEffect != null)
            confettiEffect.Play();

        if (cheerAudio != null)
            cheerAudio.Play();
    }
}
