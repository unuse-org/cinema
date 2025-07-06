using UnityEngine;

/* データ初期化プログラム

定義場所：タイトル画面
内容：

    呼ばれた時に、スコア、シーン番号を初期化する。
    また、ランダムに曜日を一つ定義する。

*/
public class TitleDataInitializer : MonoBehaviour
{
    void Start()
    {
        // PlayerPrefsで保存されているデータを確認
        if (!PlayerPrefs.HasKey("weekday"))
        {
            // データがない場合、新規作成
            Debug.Log("PlayerPrefsにデータが存在しないため、新規作成します");

            int weekday = Random.Range(0, 5);  // 0〜4の乱数
            int index = 1;
            int score = 15;

            // PlayerPrefsにデータを保存
            PlayerPrefs.SetInt("weekday", weekday);
            PlayerPrefs.SetInt("index", index);
            PlayerPrefs.SetInt("score", score);

            // 保存を確定
            PlayerPrefs.Save();

            //Debug.Log("PlayerPrefs初期化完了");
        }
        else
        {
            // PlayerPrefsが既に存在している場合、中身を空にして再初期化
            //Debug.Log("PlayerPrefsは既に存在しています。中身を初期化します");

            int weekday = Random.Range(1, 6);  // 1〜5の乱数
            int index = 1;
            int score = 15;

            // 中身を初期化して再設定
            PlayerPrefs.SetInt("weekday", weekday);
            PlayerPrefs.SetInt("index", index);
            PlayerPrefs.SetInt("score", score);
            // Debug.Log(score);

            // 保存を確定
            PlayerPrefs.Save();

            //Debug.Log("PlayerPrefsの中身を初期化しました");
        }
    }
}
