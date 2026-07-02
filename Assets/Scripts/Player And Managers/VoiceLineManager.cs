using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VoiceLineManager : MonoBehaviour
{
    public TMP_Text VoiceLineMessage;

    public GameObject VoiceLinePanel;

    List<string> Sentences;
    List<AudioClip> Clips;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Sentences = new List<string>();
        Clips = new List<AudioClip>();
        VoiceLinePanel.SetActive(false);
    }

    public void StartVoiceLine(List<string> snt, List<AudioClip> clips, StoryEvent story) {
        Sentences.Clear();
        Clips.Clear();

        Sentences.AddRange(snt);
        Clips.AddRange(clips);

        StartCoroutine(PlayVoiceLine(story));
    }

    IEnumerator PlayVoiceLine(StoryEvent story) {
        VoiceLinePanel.SetActive(true);

        for(int i = 0; i < Sentences.Count; i++) {
            StartCoroutine(TypeSentence(VoiceLineMessage, Sentences[i]));
            GameManager.Instance.GetAudioManager().PlayVoiceLine(Clips[i]);
            yield return new WaitForSeconds(Clips[i].length + 0.5f);
        }

        if(story != null) story.EngageEvent();

        VoiceLinePanel.SetActive(false);
    }

    IEnumerator TypeSentence(TMP_Text text, string sentence) {
        text.text = "";

        foreach (char letter in sentence.ToCharArray()) {
            text.text += letter;
            yield return new WaitForSeconds(.05f);
        }
    }
}
