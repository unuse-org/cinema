using UnityEngine;

public class TitleDataInitializer : MonoBehaviour
{
    void Start()
    {
        // PlayerPrefsで保存されているデータを確認
        if (!PlayerPrefs.HasKey("weekday"))
        {
            // データがない場合、新規作成
            Debug.Log("PlayerPrefsにデータが存在しないため、新規作成します");

            int weekday = Random.Range(1, 6);  // 1〜5の乱数
            int index = 1;
            int score = 0;

            // PlayerPrefsにデータを保存
            PlayerPrefs.SetInt("weekday", weekday);
            PlayerPrefs.SetInt("index", index);
            PlayerPrefs.SetInt("score", score);

            // 保存を確定
            PlayerPrefs.Save();

            Debug.Log("PlayerPrefs初期化完了");
        }
        else
        {
            // PlayerPrefsが既に存在している場合、中身を空にして再初期化
            Debug.Log("PlayerPrefsは既に存在しています。中身を初期化します");

            int weekday = Random.Range(1, 6);  // 1〜5の乱数
            int index = 1;
            int score = 0;

            // 中身を初期化して再設定
            PlayerPrefs.SetInt("weekday", weekday);
            PlayerPrefs.SetInt("index", index);
            PlayerPrefs.SetInt("score", score);

            // 保存を確定
            PlayerPrefs.Save();

            Debug.Log("PlayerPrefsの中身を初期化しました");
        }
    }
}
