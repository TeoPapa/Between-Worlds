using UnityEngine;

public class ObjectID : MonoBehaviour
{
    [SerializeField] private string ID;

    protected bool HasEngaged = false;
    protected bool CanSave = false;

    public ObjectID(string id) {
        ID = id;
    }
    public string GetID() => ID;

    public virtual void Engaged() {
        if(CanSave) GameManager.Instance.Save();
        HasEngaged = true;
        GameHandler.AddObject(this.ID);
        Destroy(gameObject);
    }

    public override bool Equals(object other) {
        ObjectID id = other as ObjectID;

        if (id == null) return false;

        if (id.GetID() == ID) return true;

        return false;
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(ID))
        {
            ID = System.Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }

#endif
}
