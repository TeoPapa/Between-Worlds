using UnityEngine;

[RequireComponent(typeof(Dialogue))]
public class NPC : Interaction {
    private Dialogue DialogueData;
    public bool DestroysOnEnd = false;

    public NPC(string id) : base(id) {
    }

    private void Start() {
        DialogueData = GetComponent<Dialogue>();
    }

    public override void InteractAction() {
        DialogueData.Converse();
    }

    protected override void EndInteraction() {
        if(DestroysOnEnd)
            Destroy(gameObject);
        base.EndInteraction();
    }
}
