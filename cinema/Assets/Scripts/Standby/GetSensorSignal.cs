using UnityEngine;
using TMPro;
using System.Collections;

/*加速度センサー処理プログラム

    加速度センサーからの値を受け付けるプログラム
    シーン移動のメインシステムでもある

定義場所：スタンバイシーン
内容：

    加速度センサーの値に応じて、画面全体にUIを表示する
    上映までのステップを用意し、すべてのステップが完了次第、メインシーンに移動
    シーン移動する際、スコアを計算し、保存する


*/

public class GetSensorSignal : MonoBehaviour
{
    // 現在の状態と直前の状態を保持
    private int currentState;
    private int previousState = 0;

    // ステップ通過フラグ
    private bool step1Passed = false;
    private bool step2Passed = false;
    private float step2Timer = 0f;

    // 状態を表示するTextMeshProコンポーネント
    [SerializeField] private TMP_Text statusText;
    public udp_receiver_imu imuReceiver; 

    // 現在動作中のフェードアウト用コルーチン
    private Coroutine fadeCoroutine;

    private int people;
    private int index;
    public GetColorSensorSignalWifi getColorSensorSignalWifi;

    private ChangeSceneManager changeSceneManager;
    //[SerializeField] private SceneChanger sceneChanger;

    private udp_receiver_imu receiver_imu;

    // 各状態番号に対応するメッセージ（インデックス1〜5を使用）
    private string[] stateMessages = {
        "", // index 0は未使用
        "映写",    // 1
        "フィルムそう入",                // 2
        "停止",                  // 3
        "逆転",             // 4
        "逆転映写"                // 5
    };

    void Awake()
    {
        GameObject imuObject = GameObject.Find("M5_IMU_Wifi");
        if (imuObject != null)
        {
            receiver_imu = imuObject.GetComponent<udp_receiver_imu>();
            if (receiver_imu == null)
            {
                Debug.LogError("udp_receiver_imu スクリプトが M5_IMU_Speed_Wifi にアタッチされていません。");
            }
        }
    }

    void Start()
    {
        people = PlayerPrefs.GetInt("people");       // デフォルト値を0
        index = PlayerPrefs.GetInt("index");
    }

    // 毎フレーム実行
    void Update()
    {
        //UDPから取得
        // currentState = receiver_imu.senser;
        // // デバッグ用メッセージ出力
        // string message = $"[Input] State: {currentState} - {stateMessages[currentState]}";
        // //Debug.Log(message);
        // // テキストがアサインされている場合、状態名を表示
        // if (statusText != null)
        // {
        //     statusText.text = stateMessages[currentState];
        //     statusText.alpha = 1f;

        //     // フェード中なら停止してから再スタート
        //     if (fadeCoroutine != null)
        //         StopCoroutine(fadeCoroutine);

        //     fadeCoroutine = StartCoroutine(FadeOutText(statusText, 2f)); // 2秒かけてフェードアウト
        // }

        GetInput();            // キー入力を取得
        //Debug.Log("現在の入力： "+currentState);
        CheckStateSequence();  // ステップ判定処理
    }

    // キー入力（1〜5）を受け取り、現在の状態を更新
    // ☑️太田「デバッグ用なのでセンサー処理でき次第変更」
    void GetInput()
    {
        for (int i = 1; i <= 5; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                previousState = currentState;
                currentState = i;

                // デバッグ用メッセージ出力
                string message = $"[Input] State: {currentState} - {stateMessages[currentState]}";
                Debug.Log(message);

                // テキストがアサインされている場合、状態名を表示
                if (statusText != null)
                {
                    statusText.text = stateMessages[currentState];
                    statusText.alpha = 1f;

                    // フェード中なら停止してから再スタート
                    if (fadeCoroutine != null)
                        StopCoroutine(fadeCoroutine);

                    fadeCoroutine = StartCoroutine(FadeOutText(statusText, 2f)); // 2秒かけてフェードアウト
                }
            }
        }
        
    }

    // 入力された状態が正しい順序かを確認し、ステップ進行またはリセット
    // ☑️太田「ここがメインシステム。ステップが完了次第mainシーンに移動する」
    void CheckStateSequence()
    {

        getColorSensorSignalWifi.UpdateSensorSignal();


        // ステップ1：Stop（3）が入力されたら通過
        if (!step1Passed && currentState == 3)
        {
            step1Passed = true;
        }

        // ステップ2：1が1秒間入力され続けたら通過
        if (step1Passed && !step2Passed)
        {
            if (currentState == 1)
            {
                step2Timer += Time.deltaTime;

                if (step2Timer >= 1f)
                {
                    step2Passed = true;

                    // 完了メッセージを表示
                    if (statusText != null)
                    {
                        //statusText.text = "Completed!";
                        statusText.alpha = 1f;

                        if (fadeCoroutine != null)
                            StopCoroutine(fadeCoroutine);

                        fadeCoroutine = StartCoroutine(FadeOutText(statusText, 2f));
                    }

                    // ☑️太田スコア計算関数へ移動
                    scorecount();
                }
            }
            else
            {
                // 1以外が入ったらタイマーをリセット
                step2Timer = 0f;
            }
        }
    }

    // ステップ進行の状態を初期化
    void ResetSteps()
    {
        step1Passed = false;
        step2Passed = false;
        //Debug.Log("🔄 Steps reset.");
    }

    // 指定のTextMeshProテキストをduration秒で透明にフェードアウト
    // ☑️太田「デザインの話。システムとは無関係」
    IEnumerator FadeOutText(TMP_Text tmpText, float duration)
    {
        float elapsed = 0f;
        float startAlpha = 1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            tmpText.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration); // 線形補間で徐々に透明化
            yield return null;
        }

        tmpText.alpha = 0f; // 最後に完全に非表示
    }

    void scorecount()
    {
        ResetSteps(); // ステップ状態を初期化

        GetColorSensorSignalWifi sensorScript = GetComponent<GetColorSensorSignalWifi>();
        bool result = sensorScript.CheckSchedule();
        if (result)
        {
            Debug.Log("✅ スケジュールが一致しました！");
            people += 2;
        }
        else
        {
            Debug.Log("❌ スケジュールが一致しません！");
            people -= 2;
        }
        //スコア計算処理＆index関数の更新
        index += 1;
        PlayerPrefs.SetInt("index", index);
        PlayerPrefs.SetInt("people", people);
        PlayerPrefs.Save();

        // シーン内にあるSceneManagerという名前のオブジェクトを探して、SceneChangerコンポーネントを取得
        GameObject sceneManagerObj = GameObject.Find("SceneManager");
        if (sceneManagerObj != null)
        {
            changeSceneManager = sceneManagerObj.GetComponent<ChangeSceneManager>();
            StartCoroutine(ProcessRoutine());
        }
        else
        {
            Debug.LogError("SceneManager オブジェクトが見つかりません");
        }
    }
    // コルーチン：LightDimmerの終了を待ってから処理を再開
    private IEnumerator ProcessRoutine()
    {
        Debug.Log("暗転開始...");

        LightDimmer dimmer = FindObjectOfType<LightDimmer>();
        if (dimmer != null)
        {
            // dimmingが終わるまで処理を待つ
            yield return StartCoroutine(dimmer.StartDimming());
        }
        else
        {
            Debug.LogWarning("LightDimmerが見つかりません。");
            yield break;
        }

        // 暗転後の処理をここに書く
        Debug.Log("暗転終了。次の処理開始！");
        // 例えば：BGM再生や敵出現など
        //ここでシーン移動実行
        changeSceneManager.LoadScene();
    }
}
