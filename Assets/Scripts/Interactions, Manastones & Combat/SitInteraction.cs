using UnityEngine;

public class SitInteraction : Interaction {

    public float Angle;

    public SitInteraction(string id) : base(id) {
    }

    public override void InteractAction() {
        PlayerMovement mv = GameManager.Instance.GetMovement();

        if (!mv.isSitting) {
            float angle = this.gameObject.transform.rotation.y;
            if(Angle != 0f)
                angle += Angle;
            
            mv.Sit(angle);
            return;
        }

        mv.Stand();
    }
}
