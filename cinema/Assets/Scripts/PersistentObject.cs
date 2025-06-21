using UnityEngine;

public class PersistentObject : MonoBehaviour
{
    public string objectId;
    public ObjectStateDatabase stateDatabase;

    // 状態確認メソッドを追加
    public void CheckState()
    {
        bool savedState = stateDatabase.GetState(objectId); // 保存された状態を取得
        gameObject.SetActive(savedState); // 状態を反映

        // アクティブな場合にデバッグメッセージを出力
        if (savedState)
        {
            //Debug.Log($"[Human状態確認] {objectId} はアクティブで生存しています。");
        }
    }
}
