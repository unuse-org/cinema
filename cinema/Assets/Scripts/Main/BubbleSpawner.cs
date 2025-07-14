using UnityEngine;
using System.Collections.Generic;

public class BubbleSpawner : MonoBehaviour
{
    public static BubbleSpawner Instance;

    // ───────────────────────────────── 共通設定
    [Header("共通設定")]
    [SerializeField] private GameObject bubblePrefab;         // 空の GameObject プレハブ（SpriteRenderer を持たせる）
    [SerializeField] private GameObject[] humans = new GameObject[35];

    // ───────────────────────────────── PNG セット（各3枚）
    [Header("吹き出し PNG（速い／遅い／成功 各3枚）")]
    [SerializeField] private Texture2D[] speedUpTextures   = new Texture2D[3];
    [SerializeField] private Texture2D[] speedDownTextures = new Texture2D[3];
    [SerializeField] private Texture2D[] successTextures   = new Texture2D[3];

    // 呼び出し用の状況
    public enum Situation { SpeedUp, SpeedDown, Success }

    // Runtime 生成用
    private Dictionary<Situation, Sprite[]> spriteDict;

    // ──────────────────────────────── 初期化
    private void Awake()
    {
        if (Instance == null) Instance = this;

        spriteDict = new Dictionary<Situation, Sprite[]>
        {
            { Situation.SpeedUp,   ConvertTexturesToSprites(speedUpTextures)   },
            { Situation.SpeedDown, ConvertTexturesToSprites(speedDownTextures) },
            { Situation.Success,   ConvertTexturesToSprites(successTextures)   }
        };
    }

    /// <summary>Texture2D 配列 → Sprite 配列に変換</summary>
    private Sprite[] ConvertTexturesToSprites(Texture2D[] textures)
    {
        if (textures == null) return new Sprite[0];

        Sprite[] sprites = new Sprite[textures.Length];
        for (int i = 0; i < textures.Length; i++)
        {
            Texture2D tex = textures[i];
            if (tex == null) continue;

            sprites[i] = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f),
                100f); 
        }
        return sprites;
    }

    public void SpawnBubbles(Situation situation)
    {
        // プレハブ & Canvas チェック
        if (bubblePrefab == null) return;

        // スプライト配列取得
        if (!spriteDict.TryGetValue(situation, out Sprite[] sprites) ||
            sprites == null || sprites.Length == 0)
        {
            Debug.LogError($"❌ {situation} 用のスプライトが設定されていません");
            return;
        }

        // アクティブ Human 収集（すべてのHumanを対象にする）
        List<GameObject> allHumans = new List<GameObject>();
        foreach (var h in humans)
        {
            if (h != null)
                allHumans.Add(h);
        }

        if (allHumans.Count == 0)
        {
            Debug.LogWarning("⚠️ humans 配列に有効なオブジェクトがありません");
            return;
        }

        // 吹き出し生成処理
        // 吹き出し生成処理
        foreach (Sprite sp in sprites)
        {
            GameObject target = allHumans[Random.Range(0, allHumans.Count)];
            if (target == null)
            {
                Debug.LogWarning("⚠️ target が null です");
                continue;
            }

            Vector3 worldPos = target.transform.position + new Vector3(-0.5f, 5.6f, 0f);

            GameObject bubble = new GameObject("BubbleImage");
            bubble.transform.position = worldPos;

            SpriteRenderer spriteRenderer = bubble.AddComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = sp;

                // ✅ 半透明化
                Color color = spriteRenderer.color;
                color.a = 0.2f; 
                spriteRenderer.color = color;
            }
        }

    }
}
