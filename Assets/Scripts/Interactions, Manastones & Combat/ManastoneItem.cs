using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ManastoneItem : ObjectID
{
    public Animator animator;
    public List<GameObject> EnablingObjects;
    public List<GameObject> DeletedObjects;

    public ManastoneItem(string id) : base(id) {
    }

    public void Activate() {
        Engaged();
    }

    public void ManastoneAction() {
        foreach (GameObject obj in EnablingObjects) {
            obj.SetActive(true);
        }
        foreach (GameObject obj in DeletedObjects) {
            Destroy(obj);
        }
    }

    private void Awake() {
        foreach(GameObject obj in EnablingObjects) {
            obj.SetActive(false);
        }
    }

    private void Start() {
        animator.SetTrigger("Start");
    }

    public override void Engaged() {
        if(HasEngaged) return;

        animator.SetTrigger("Activate");
    }
}
