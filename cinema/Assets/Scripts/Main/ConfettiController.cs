using UnityEngine;

public class ConfettiController : MonoBehaviour
{
<<<<<<< HEAD
    // Start is called once before the first execution of Update after the MonoBehaviour is created
=======
>>>>>>> main
    public ParticleSystem[] confettiSystems;  // 6つのParticleSystemをここに登録

    public void PlayOnce()
    {
        foreach (var ps in confettiSystems)
        {
            if (ps != null)
            {
                var main = ps.main;
                main.loop = false;
                // main.stopAction = confettiSystems.Stop;
                ps.Play();
                StartCoroutine(StopAfter(ps, 3f));
            }
        }
    }

    private System.Collections.IEnumerator StopAfter(ParticleSystem ps, float time)
    {
        yield return new WaitForSeconds(time);
        ps.Stop();
    }

<<<<<<< HEAD
=======
    // // 再生する
    // public void PlayConfetti()
    // {
    //     if (confetti != null && !confetti.isPlaying)
    //     {
    //         confetti.Play();
    //     }
    // }

    // // 停止する
    // public void StopConfetti()
    // {
    //     if (confetti != null && confetti.isPlaying)
    //     {
    //         confetti.Stop();
    //     }
    // }

    // // ループを有効化する（必要な場合）
    // public void SetLoop(bool isLooping)
    // {
    //     if (confetti != null)
    //     {
    //         var main = confetti.main;
    //         main.loop = isLooping;
    //     }
    // }
>>>>>>> main
}
