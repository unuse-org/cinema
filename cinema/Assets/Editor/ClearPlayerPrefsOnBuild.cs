using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class ClearPlayerPrefsOnBuild : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    void Awake()
    {
        // 初回起動フラグが存在しなければ（＝初回起動の場合）
        if (!PlayerPrefs.HasKey("FirstRunDone"))
        {
            Debug.Log("初回起動検出：PlayerPrefs をリセットします。");

            // 全データ削除
            PlayerPrefs.DeleteAll();

            // 初回起動済みフラグを保存
            PlayerPrefs.SetInt("FirstRunDone", 1);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("2回目以降の起動：PlayerPrefs は保持されます。");
        }
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs cleared before build!");
    }
}