using UnityEngine;

public class Option : MonoBehaviour
{
    public int Jump;

    public void Selected() {
        DialogueManager manager = GameManager.Instance.GetDialogueManager();

        manager.setIndex(Jump);
        manager.DisplayNextSentence();
    }

    public void SetOption(int jump, string text) {
        Jump = jump;
        this.GetComponentInChildren<TMPro.TMP_Text>().text = text;
    }
}
