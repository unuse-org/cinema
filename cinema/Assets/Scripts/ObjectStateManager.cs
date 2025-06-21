using UnityEngine;
using System.Collections.Generic;
using System.Collections; // ← これが必要
using TMPro;

public class ObjectStateManager : MonoBehaviour
{
    [SerializeField] private ObjectStateDatabase stateDatabase;
    [SerializeField] private string objectIdPrefix = "human"; // オブジェクト名の接頭辞
    [SerializeField] private int objectCount = 35;  // 管理するオブジェクト数

    [SerializeField] private TextMeshProUGUI scoreText;

    private List<string> objectIds = new List<string>();  // オブジェクトIDのリスト
    private List<GameObject> managedObjects = new List<GameObject>();  // 管理対象のオブジェクトリスト

    void Start()
    {
        InitializeAllStatesToTrue(); // 最初にすべてTrueにする
        // シーン開始時にオブジェクト状態を初期化
        InitializeObjectStates();
    }

    void InitializeAllStatesToTrue()
    {
        for (int i = 0; i < objectCount; i++)
        {
            string objectId = objectIdPrefix + i;  // 例: "human0", "human1", ...
            stateDatabase.SetState(objectId, true);
        }
    }

    void InitializeObjectStates()
    {
        int score = PlayerPrefs.GetInt("score", 0);

        //int score = 15;
        
        managedObjects = new List<GameObject>();  // managedObjects が null であれば初期化

       for (int i = 0; i < objectCount; i++)
        {
            string objectId = objectIdPrefix + i;  // オブジェクトのID（human0, human1, ...）
            GameObject obj = GameObject.Find(objectId);

            if (obj != null)
            {
                managedObjects.Add(obj);  // オブジェクトをリストに追加
                objectIds.Add(objectId);  // 🔺ここを追加！
                //Debug.Log($"オブジェクト {objectId} を追加しました");
            }
            else
            {
                Debug.LogWarning($"オブジェクト {objectId} が見つかりません");
            }
        }

        // アクティブなオブジェクトのインデックスを収集
            List<int> activeIndices = new();
        for (int i = 0; i < managedObjects.Count; i++)
        {
            //Debug.Log("探索してます");
            // managedObjects[i] がアクティブかつシーン内で有効か
            if (managedObjects[i].activeInHierarchy)
            {
                activeIndices.Add(i);
            }
        }

        // 差分がある場合のみランダムに非アクティブ化
        int currentActive = activeIndices.Count;
        int diff = currentActive - score;
        //Debug.Log($"[検証] score = {score}, アクティブなHuman数 = {currentActive}, 差分 = {diff}");

        if (diff > 0)
        {
            // アクティブが多すぎる → ランダムに非アクティブにする
            int deactivateCount = Mathf.Min(diff, activeIndices.Count);  // 安全な数だけ
            for (int i = 0; i < deactivateCount; i++)
            {
                int randomIndex = Random.Range(0, activeIndices.Count);
                int objIndex = activeIndices[randomIndex];

                managedObjects[objIndex].SetActive(false);
                stateDatabase.SetState(objectIds[objIndex], false);

                activeIndices.RemoveAt(randomIndex);  // 使用済みは削除
            }
        }
        else if (diff < 0)
        {
            // アクティブが少なすぎる → ランダムにアクティブ化
            List<int> inactiveIndices = new();
            for (int i = 0; i < managedObjects.Count; i++)
            {
                if (!managedObjects[i].activeInHierarchy)
                {
                    inactiveIndices.Add(i);
                }
            }

            int activateCount = Mathf.Min(-diff, inactiveIndices.Count);  // 安全な数だけ
            for (int i = 0; i < activateCount; i++)
            {
                int randomIndex = Random.Range(0, inactiveIndices.Count);
                int objIndex = inactiveIndices[randomIndex];

                managedObjects[objIndex].SetActive(true);
                stateDatabase.SetState(objectIds[objIndex], true);

                inactiveIndices.RemoveAt(randomIndex);  // 使用済みは削除
            }
        }

        // 状態を保存した後に PersistentObject の状態を確認
        StartCoroutine(ExecuteAfterStateSetup());
    }

    // 状態設定後に少し待ってから PersistentObject の処理を実行
    private IEnumerator ExecuteAfterStateSetup()
    {
        // 少し待つことで ObjectStateManager の処理が終了するのを待つ
        yield return null; // 次のフレームで実行

        // 次のフレームで PersistentObject を実行（状態が確実に設定される）
        var persistentObjects = FindObjectsOfType<PersistentObject>();
        foreach (var persistentObject in persistentObjects)
        {
            persistentObject.CheckState();  // 状態確認メソッドを実行
        }
    }

    // スコアとの差分に応じてオブジェクトの状態を変更
    void ApplyDiffBasedActivation()
    {
        int score = PlayerPrefs.GetInt("score", 0);
        List<int> activeIndices = new();
        List<int> inactiveIndices = new();

        // アクティブと非アクティブのオブジェクトを分類
        for (int i = 0; i < managedObjects.Count; i++)
        {
            if (managedObjects[i].activeSelf)
                activeIndices.Add(i);
            else
                inactiveIndices.Add(i);
        }

        int currentActiveCount = activeIndices.Count;
        int diff = score - currentActiveCount;

        //Debug.Log($"[Debug] 現在アクティブな数（差分処理前）: {currentActiveCount}");
        //Debug.Log($"[Debug] スコア = {score}, 差分 = {diff}");

        // アクティブにするオブジェクト数が足りない場合
        if (diff > 0)
        {
            for (int i = 0; i < diff && inactiveIndices.Count > 0; i++)
            {
                int rand = Random.Range(0, inactiveIndices.Count); // 非アクティブからランダムに選択
                int objIndex = inactiveIndices[rand];

                managedObjects[objIndex].SetActive(true); // アクティブにする
                stateDatabase.SetState(objectIds[objIndex], true); // 状態を保存
                inactiveIndices.RemoveAt(rand); // 選択したオブジェクトをリストから削除
            }
        }
        // 非アクティブにするオブジェクト数が多すぎる場合
        else if (diff < 0)
        {
            int toDeactivate = -diff;
            for (int i = 0; i < toDeactivate && activeIndices.Count > 0; i++)
            {
                int rand = Random.Range(0, activeIndices.Count); // アクティブからランダムに選択
                int objIndex = activeIndices[rand];

                managedObjects[objIndex].SetActive(false); // 非アクティブにする
                stateDatabase.SetState(objectIds[objIndex], false); // 状態を保存
                activeIndices.RemoveAt(rand); // 選択したオブジェクトをリストから削除
            }
        }
    }
}
