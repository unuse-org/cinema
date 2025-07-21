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
        int weekday = Random.Range(0, 5);  // 1〜5の乱数
        // int index = 1;
        // int score = 0;
        // int people = 2;

        // 中身を初期化して再設定
        PlayerPrefs.SetInt("weekday", weekday);
        PlayerPrefs.SetInt("index", 1);
        PlayerPrefs.SetInt("Game_Score_Current", 0);
        PlayerPrefs.SetInt("people", 2);

        Debug.Log("スコア初期化後" + PlayerPrefs.GetInt("Game_Score_Current"));
        PlayerPrefs.Save();       
    }
}
