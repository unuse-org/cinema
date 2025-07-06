using UnityEngine;

public class SurpriseHumansManager : MonoBehaviour
{
    private const int HumanCount = 35;
    private const string HumanPrefix = "human"; // オブジェクト名の接頭辞（例: human0〜human34）

    public void SurpriseAllHumans()
    {
        for (int i = 0; i < HumanCount; i++)
        {
            string objectName = HumanPrefix + i;
            GameObject go = GameObject.Find(objectName);

            if (go == null)
            {
                //Debug.LogWarning($"オブジェクト {objectName} が見つかりませんでした。");
                continue;
            }

            if (!go.activeInHierarchy) continue;

            HumanBehavior hb = go.GetComponent<HumanBehavior>();
            if (hb != null)
            {
                hb.Surprise();
            }
            else
            {
                //Debug.LogWarning($"{objectName} に HumanBehavior スクリプトが見つかりませんでした。");
            }
        }
    }
}
