using UnityEngine;
using System.Collections;

public class HumanBehavior : MonoBehaviour
{
    [SerializeField] private float bounceHeight = 0.1f; // 振動の高さ
    [SerializeField] private float bounceSpeed = 0.05f; // 振動の速さ
    [SerializeField] private int bounceCount = 12;       // 上下回数（偶数で1セット）

    private bool isBouncing = false;

    public void Surprise()
    {
        if (!isBouncing)
        {
            StartCoroutine(BounceRoutine());
        }
    }

    private IEnumerator BounceRoutine()
    {
        isBouncing = true;
        Vector3 originalPosition = transform.localPosition;

        for (int i = 0; i < bounceCount; i++)
        {
            float direction = (i % 2 == 0) ? 1f : -1f; // 交互に上下
            Vector3 offset = new Vector3(0f, bounceHeight * direction, 0f);
            transform.localPosition = originalPosition + offset;

            yield return new WaitForSeconds(bounceSpeed);
        }

        // 最後に元の位置へ戻す
        transform.localPosition = originalPosition;
        isBouncing = false;
    }
}
