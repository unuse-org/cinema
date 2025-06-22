using UnityEngine;
using System.Collections.Generic;

public class SceneBObjectStateManager : MonoBehaviour
{
    [SerializeField] private ObjectStateDatabase stateDatabase;
    [SerializeField] private string objectIdPrefix = "human";
    [SerializeField] private int objectCount = 35;

    private List<GameObject> managedObjects = new();
    private List<string> objectIds = new();

    void Start()
    {
        // 1. オブジェクトの取得と状態の反映
        for (int i = 0; i < objectCount; i++)
        {
            string id = objectIdPrefix + i;
            objectIds.Add(id);

            GameObject obj = GameObject.Find(id);
            if (obj != null)
            {
                bool savedState = stateDatabase.GetState(id);
                obj.SetActive(savedState); // Aシーンで保存された状態を反映
                managedObjects.Add(obj);
            }
            else
            {
                Debug.LogWarning($"[SceneB] {id} が見つかりません");
            }
        }

        // 2. 差分処理
        ApplyDiffBasedActivation();
    }

    void ApplyDiffBasedActivation()
    {
        int score = PlayerPrefs.GetInt("score", 0);

        // int score = 18;

        List<int> activeIndices = new();
        List<int> inactiveIndices = new();

        for (int i = 0; i < managedObjects.Count; i++)
        {
            if (managedObjects[i].activeInHierarchy)
                activeIndices.Add(i);
            else
                inactiveIndices.Add(i);
        }

        int currentActive = activeIndices.Count;
        int diff = score - currentActive;

        //Debug.Log($"[SceneB] score = {score}, 現在アクティブ = {currentActive}, 差分 = {diff}");

        if (diff > 0)
        {
            int activateCount = Mathf.Min(diff, inactiveIndices.Count);
            for (int i = 0; i < activateCount; i++)
            {
                int rand = Random.Range(0, inactiveIndices.Count);
                int index = inactiveIndices[rand];

                managedObjects[index].SetActive(true);
                stateDatabase.SetState(objectIds[index], true);
                //Debug.Log($"[SceneB] {objectIds[index]} をアクティブにしました");

                inactiveIndices.RemoveAt(rand);
            }
        }
        else if (diff < 0)
        {
            int deactivateCount = Mathf.Min(-diff, activeIndices.Count);
            for (int i = 0; i < deactivateCount; i++)
            {
                int rand = Random.Range(0, activeIndices.Count);
                int index = activeIndices[rand];

                managedObjects[index].SetActive(false);
                stateDatabase.SetState(objectIds[index], false);
                //Debug.Log($"[SceneB] {objectIds[index]} を非アクティブにしました");

                activeIndices.RemoveAt(rand);
            }
        }
    }
}
