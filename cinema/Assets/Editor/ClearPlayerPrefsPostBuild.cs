using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class ClearPlayerPrefsPostBuild : IPostprocessBuildWithReport
{
    // 実行順序（複数ある場合に制御可能、0でOK）
    public int callbackOrder => 0;

    // ビルド完了後に呼ばれる
    public void OnPostprocessBuild(BuildReport report)
    {
        Debug.Log("🏗️ Build completed for: " + report.summary.outputPath);
        Debug.Log("🧹 Clearing PlayerPrefs in Editor environment...");

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Debug.Log("✅ PlayerPrefs successfully cleared after build!");
    }
}
