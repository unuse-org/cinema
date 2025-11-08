using UnityEngine;

public class ClearPlayerPrefsOnFirstRun : MonoBehaviour
{
    private const string FirstRunKey = "HasClearedPlayerPrefs";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ClearPlayerPrefsIfFirstRun()
    {
        if (!PlayerPrefs.HasKey(FirstRunKey))
        {
            Debug.Log("🧹 First launch detected! Clearing all PlayerPrefs...");

            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt(FirstRunKey, 1); // 再度削除されないようフラグを保存
            PlayerPrefs.Save();

            Debug.Log("✅ PlayerPrefs cleared successfully on first launch.");
        }
        else
        {
            Debug.Log("🔁 Not first launch. PlayerPrefs will be kept.");
        }
    }
}
