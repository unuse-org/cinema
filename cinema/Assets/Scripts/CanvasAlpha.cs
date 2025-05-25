using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class CanvasAlpha : MonoBehaviour
{
    public TextDisplay textDisplay; // TextDisplayスクリプトの参照
    void Start()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

        if (textDisplay == null)
        {
            textDisplay = FindObjectOfType<TextDisplay>();
            if (textDisplay == null)
            {
                Debug.LogError("TextDisplay script not found in the scene.");
            }
        }
    }

    void Update()
    {
        if (textDisplay.EndTextDisplayFlag == 1)
        {
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                // アルファ値を徐々に0にする
                StartCoroutine(FadeOutCanvas(canvasGroup, 0.7f)); // 1秒かけてフェードアウト
            }
            else
            {
                Debug.LogWarning("CanvasGroup component not found on the GameObject.");
            }
        }
    }
    IEnumerator FadeOutCanvas(CanvasGroup canvasGroup, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, time / duration);
            yield return null;
        }

        canvasGroup.alpha = 0f; 
    }
}
