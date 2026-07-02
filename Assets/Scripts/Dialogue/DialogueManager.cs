using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text SpeakerText;
    public TMP_Text MessageText;

    public GameObject OptionPrefab;

    public GameObject OptionParent;

    public Animator DialogueAnimator;
    public GameObject ButtonArrow;

    public GameObject ButtonObject;
    public GameObject OptionsObject;

    public GameObject Panel;

    int Index;

    bool isSpeaking;

    List<DialogueType> Sentences;

    DialogueType CurrentSentece;

    Coroutine routine;

    private void Start() {
        Sentences = new List<DialogueType>();
        DialogueAnimator.SetBool("IsSpeaking", false);
        SpeakerText.gameObject.SetActive(false);
        MessageText.gameObject.SetActive(false);
        OptionsObject.SetActive(false);
        ButtonObject.SetActive(false);
        Panel.SetActive(false);
    }

    public void StartDialogue(List<DialogueType> sentences) {
        GameManager.Instance.GetMovement().LockInput();
        Panel.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Sentences.Clear();
        Sentences.AddRange(sentences);
        Index = 0;
        StopAllCoroutines();
        StartCoroutine(WaitForAnimation(1.5f, "Open", true));
        isSpeaking = true;
        ButtonArrow.SetActive(false);
    }

    IEnumerator WaitForAnimation(float Amount, string TriggerName, bool Activate) {
        DialogueAnimator.SetTrigger(TriggerName);
        yield return new WaitForSeconds(Amount);

        if(Activate) {
            SpeakerText.gameObject.SetActive(true);
            MessageText.gameObject.SetActive(true);
            DisplayNextSentence();
        }else {
            Panel.SetActive(false);
        }
    }

    public void DisplayNextSentence() {
        ButtonArrow.SetActive(false);
        if (Sentences.Count == 0 || Index >= Sentences.Count) {
            EndDialogue();
            return;
        }

        CurrentSentece = Sentences[Index];

        SpeakerText.text = CurrentSentece.GetSpeaker();

        if (CurrentSentece.OptionCount > 0) {
            MessageText.text = "";
            OptionsObject.SetActive(true);
            ButtonObject.SetActive(false);
            OptionParent.GetComponent<RectTransform>().sizeDelta = new Vector2(OptionParent.GetComponent<RectTransform>().sizeDelta.x, CurrentSentece.OptionCount * 50);
            for(int i = 0; i < CurrentSentece.OptionCount; i++) {
                GameObject OptionButton = Instantiate(OptionPrefab, OptionParent.transform);
                OptionButton.GetComponent<Option>().SetOption(CurrentSentece.Jumps[i], CurrentSentece.GetOption(i));
            }
        }
        else {
            OptionsObject.SetActive(false);
            ButtonObject.SetActive(true);
            GameManager.Instance.GetAudioManager().PlayDialogue(CurrentSentece.VoiceLine);
            StopAllCoroutines();
            routine = StartCoroutine(TypeSentence(MessageText, CurrentSentece.GetMessage(), true));
            Index += 1;
        }
    }

    public void ChoseOptionAndDisplay(TMP_Text OptionString) {
        for(int i =0; i < CurrentSentece.OptionCount; i++) {
            if (OptionString.text == CurrentSentece.GetOption(i)) {
                Index = CurrentSentece.Jumps[i];
                DisplayNextSentence();
                return;
            }
        }
    }

    /* A function that makes the sentence show up letter by letter. */
    IEnumerator TypeSentence(TMP_Text text, string sentence, bool Button) {
        text.text = "";

        foreach (char letter in sentence.ToCharArray()) {
            text.text += letter;
            yield return new WaitForSeconds(.03f);
        }
        if(Button)
            ButtonArrow.SetActive(true);
    }

    public void EndDialogue() {
        foreach(Transform child in OptionParent.transform) {
            Destroy(child.gameObject);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        OptionsObject.SetActive(false);
        SpeakerText.gameObject.SetActive(false);
        MessageText.gameObject.SetActive(false);
        StopAllCoroutines();
        StartCoroutine(WaitForAnimation(1.5f, "Close", false));
        isSpeaking = false;
        GameManager.Instance.GetMovement().UnlockInput();
    }

    public void setIndex(int x) {
        Index = x;
    }
}
