using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class WalkingVoiceLine : ObjectID {

    public int[] VoiceLineIndex;
    List<string> VoiceLines;
    public List<AudioClip> VoiceLineClips;

    public StoryEvent Story;


    private void Start() {
        VoiceLines = new List<string>();
        string TableName = SceneManager.GetActiveScene().name + "VoiceLines";
        StringTable sentences = LocalizationSettings.StringDatabase.GetTable(TableName);
        foreach (int i in VoiceLineIndex) {
            string message = sentences.GetEntry(TableName + "." + i).LocalizedValue;
            VoiceLines.Add(message);
        }
    }

    public WalkingVoiceLine(string id) : base(id) {
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player")) {
            GameManager.Instance.GetVoiceLineManager().StartVoiceLine(VoiceLines, VoiceLineClips, Story);
            CanSave = true;
            if (Story != null) Story.EngageEvent();
            Engaged();
        }
    }
}
