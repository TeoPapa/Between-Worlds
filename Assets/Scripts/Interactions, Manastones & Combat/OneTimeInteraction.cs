using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class OneTimeInteraction : ObjectID {

    public List<Interaction> Interaction;

    public bool LockInput = false;

    public OneTimeInteraction(string id) : base(id) {
    }

    void OnTriggerEnter(Collider other) {
        if(!other.CompareTag("Player")) return;
        foreach(Interaction i in Interaction) {
            i.Interact();
        }

        GameManager.Instance.GetMovement().LockedInput = LockInput;
        Engaged();
    }
}
