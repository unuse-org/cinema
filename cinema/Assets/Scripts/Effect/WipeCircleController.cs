using UnityEngine;

/// <summary>
/// WipeCircleController.cs
/// このスクリプトは、指定されたマテリアルの半径をアニメーションで変化させることで、円形のワイプエフェクトを実現します。
/// </summary>
public class WipeCircleController : MonoBehaviour
{
    public Material wipeMaterial; // 対象のマテリアル
    public float duration = 2.0f; // アニメーション時間（秒）

    private bool isWiping = false;
    private float timer = 0f;

    void Start()
    {
        if (wipeMaterial == null)
        {
            Debug.LogError("Wipe material is not assigned.");
            enabled = false; // スクリプトを無効化
        }
        else
        {
            wipeMaterial.SetFloat("_Radius", 2f); // 初期状態の半径を設定
        }
    }

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