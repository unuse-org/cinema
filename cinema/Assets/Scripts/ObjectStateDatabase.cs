using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ObjectStateDatabase", menuName = "Game/ObjectStateDatabase")]
public class ObjectStateDatabase : ScriptableObject
{
    [System.Serializable]
    public class ObjectState
    {
        public string id;       // 各オブジェクトに一意なID（例: "obj_01"）
        public bool isActive;
    }

    public List<ObjectState> states = new List<ObjectState>();

    public void SetState(string id, bool isActive)
    {
        var obj = states.Find(s => s.id == id);
        if (obj != null)
            obj.isActive = isActive;
        else
            states.Add(new ObjectState { id = id, isActive = isActive });
    }

    public bool GetState(string id)
    {
        var obj = states.Find(s => s.id == id);
        return obj != null ? obj.isActive : false;
    }
}
