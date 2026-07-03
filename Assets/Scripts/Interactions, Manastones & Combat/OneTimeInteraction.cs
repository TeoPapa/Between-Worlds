using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(StoryEvent))]
public class OneTimeInteraction : MonoBehaviour{

    public List<Interaction> Interaction;

    public StoryEvent Story;

    private void Awake() {
        Story = this.GetComponent<StoryEvent>();
        Story.DisablingObjects.Add(this.gameObject);
    }

    void OnTriggerEnter(Collider other) {
        if(!other.CompareTag("Player")) return;
        foreach(Interaction i in Interaction) {
            i.Interact();
        }

        Story.EngageEvent();
    }
}
