using UnityEngine;

public class ConfettiController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

}
