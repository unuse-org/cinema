using UnityEngine;
using System.Collections;

public class HumanMove : MonoBehaviour
{
    [SerializeField] private GameObject[] humanPrefabs = new GameObject[6];
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float moveTimeMin = 5f;
    [SerializeField] private float moveTimeMax = 10f;
    [SerializeField] private float fadeOutTime = 10f;
    [SerializeField] private float moveSpeed = 3f;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            int index = Random.Range(0, humanPrefabs.Length);
            GameObject selectedPrefab = humanPrefabs[index];

            if (selectedPrefab != null)
            {
                // 回転の条件：index が3以上の場合 Y軸180度回転
                Quaternion rotation = (index >= 3) ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;

                GameObject copy = Instantiate(selectedPrefab, selectedPrefab.transform.position, rotation);
                StartCoroutine(MoveAndFade(copy, index));
            }
        }
    }

    private IEnumerator MoveAndFade(GameObject obj, int index)
    {
        float moveDuration = Random.Range(moveTimeMin, moveTimeMax);
        float timer = 0f;

        Vector3 direction = (index < 3) ? Vector3.right : Vector3.left;

        while (timer < moveDuration)
        {
            obj.transform.position += direction * moveSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        // 3Dオブジェクト用フェードアウト処理（Materialのアルファ値を使う）
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null && renderer.material.HasProperty("_Color"))
        {
            float fadeTimer = 0f;
            Color originalColor = renderer.material.color;

            // 透明度を下げていく
            while (fadeTimer < fadeOutTime)
            {
                float alpha = Mathf.Lerp(1f, 0f, fadeTimer / fadeOutTime);
                Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                renderer.material.color = newColor;

                fadeTimer += Time.deltaTime;
                yield return null;
            }
        }

        Destroy(obj);
    }
}
