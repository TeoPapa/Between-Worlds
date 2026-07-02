using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Interaction : ObjectID
{
    public string InteractionName;
    public abstract void InteractAction();

    public List<GameObject> EnablingObjects;
    public List<GameObject> DisablingObjects;

    public bool Save;

    protected Interaction(string id) : base(id) {
    }

    private void Awake() {
        foreach (GameObject go in EnablingObjects) {
            go.SetActive(false);
        }

           foreach (GameObject go in DisablingObjects) {
                go.SetActive(true);
        }
    }

    public void Interact() {
        InteractAction();
        CanSave = Save;
        Engaged();
    }

    public override void Engaged() {
        if(HasEngaged) return;

        foreach (GameObject go in EnablingObjects) {
            go.SetActive(true);
        }
        foreach(GameObject go in DisablingObjects) {
            go.SetActive(false);
        }

        HasEngaged = true;
        GameHandler.AddObject(this.GetID());
        EndInteraction();
    }

    protected virtual void EndInteraction() { if(CanSave) GameManager.Instance.Save(); }

    private void OnTriggerEnter(Collider other) {
        if(!other.CompareTag("Player")) return;

        other.GetComponent<PlayerInteractions>().SetInteraction(this);
    }

    private void OnTriggerExit(Collider other) {
        if(!other.CompareTag("Player")) return;

        PlayerInteractions playerInteractions = other.GetComponent<PlayerInteractions>();
        if(playerInteractions.isCurrent(this)) {
            playerInteractions.SetInteraction(null);
        }
    }
}
