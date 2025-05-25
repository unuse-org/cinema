using UnityEngine;

public class WipeCircleController : MonoBehaviour
{
    public Material wipeMaterial; // 対象のマテリアル
    public float duration = 2.0f; // アニメーション時間（秒）

    private bool isWiping = false;
    private float timer = 0f;

    void Update()
    {
        if (isWiping)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            float radius = Mathf.Lerp(2f, 0f, t);
            wipeMaterial.SetFloat("_Radius", radius);

            if (t >= 1f)
            {
                isWiping = false;
            }
        }
    }

    // ボタンから呼び出す用
    public void StartWipe()
    {
        timer = 0f;
        isWiping = true;
    }
}