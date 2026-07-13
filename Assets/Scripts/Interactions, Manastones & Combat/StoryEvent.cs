using System.Collections.Generic;
using UnityEngine;

public class StoryEvent : ObjectID
{
    public List<GameObject> EnablingObjects;
    public List<GameObject> DisablingObjects;

    public bool CombatLock;
    public bool ShadowLock;

    public bool LockedInput = false;

    public bool Save;

    public StoryEvent(string id) : base(id) {
    }

    private void Start() {
        foreach (GameObject go in EnablingObjects) {
            try { go.SetActive(false); } catch { Debug.LogWarning("One of the EnablingObjects in " + gameObject.name + " is null!"); }
        }

        foreach (GameObject go in DisablingObjects) {
            try {  go.SetActive(true); } catch { Debug.LogWarning("One of the DisablingObjects in " + gameObject.name + " is null!"); }
        }
    }

    public override void Engaged() {
        if (HasEngaged) return;

        PlayerCombat Combat = GameManager.Instance.Player.GetComponent<PlayerCombat>();
        PlayerMovement move = GameManager.Instance.Player.GetComponent<PlayerMovement>();

        if (LockedInput) {
            move.LockInput();
        } else {
            move.UnlockInput();
        }

        Combat.CombatLock = CombatLock;
        Combat.ShadowLock = ShadowLock;

        foreach (GameObject go in EnablingObjects) {
            go.SetActive(true);
        }

        foreach(GameObject go in DisablingObjects) {
            go.SetActive(false);
        }

        GameHandler.AddObject(this.GetID());
        if (CanSave) GameManager.Instance.Save();
    }

    public void EngageEvent() {
        CanSave = Save;
        this.Engaged();
    }
}
