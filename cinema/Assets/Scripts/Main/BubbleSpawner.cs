using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class BubbleSpawner : MonoBehaviour
{
    public static BubbleSpawner Instance;

    [SerializeField] private GameObject bubblePrefab;
    [SerializeField] private RectTransform uiCanvasParent;
    [SerializeField] private GameObject[] humans = new GameObject[35];
    [SerializeField] private int maxBubbleCount = 3;

    private Dictionary<string, string[]> accidentMessageDict;

    void Awake()
    {
        if (Instance == null) Instance = this;

        // メッセージを初期化（ここに直接定義）
        accidentMessageDict = new Dictionary<string, string[]>
        {
            {
                "SpeedUp", new string[]
                {
                    "速すぎ！", "ギュイーン！", "ブースト！", "急げー！", "飛ぶぞ！？"
                }
            },
            {
                "SpeedDown", new string[]
                {
                    "遅っ…", "トロトロだよ！", "牛歩戦術？", "ねむ…", "止まりそう…"
                }
            }
        };
    }

    public void SpawnBubbles(string accidentType)
    {
        if (bubblePrefab == null || uiCanvasParent == null) return;

        // アクティブなヒューマンを取得
        List<GameObject> activeHumans = new List<GameObject>();
        foreach (var human in humans)
        {
            if (human != null && human.activeInHierarchy)
                activeHumans.Add(human);
        }

        if (activeHumans.Count == 0)
        {
            Debug.Log("⚠️ アクティブなHumanがいません");
            return;
        }

        int spawnCount = Mathf.Clamp(Random.Range(1, 4), 1, Mathf.Min(maxBubbleCount, activeHumans.Count));

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject target = activeHumans[Random.Range(0, activeHumans.Count)];
            Vector3 worldPos = target.transform.position + new Vector3(0.5f, 0.5f, 0);
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                uiCanvasParent,
                screenPos,
                null,
                out Vector2 localPoint
            );

            GameObject bubble = Instantiate(bubblePrefab, uiCanvasParent);
            RectTransform rect = bubble.GetComponent<RectTransform>();
            rect.anchoredPosition = localPoint;

            string message = GetRandomMessage(accidentType);
            TextMeshProUGUI tmp = bubble.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) tmp.text = message;

            Debug.Log($"💬 吹き出し生成: 『{message}』 @ {target.name}  pos:{localPoint}");

            Destroy(bubble, 2.5f);
        }
    }

    private string GetRandomMessage(string type)
    {
        if (accidentMessageDict != null && accidentMessageDict.ContainsKey(type))
        {
            string[] messages = accidentMessageDict[type];
            return messages[Random.Range(0, messages.Length)];
        }

        return "アクシデント！";
    }
}
