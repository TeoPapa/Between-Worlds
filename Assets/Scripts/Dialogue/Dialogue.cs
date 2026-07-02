using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.UIElements;

public class Dialogue : MonoBehaviour
{
    public string TableName;

    [SerializeField]
    public List<DialogueType> Dialogues;

    void Start()
    {
        StringTable sentences = LocalizationSettings.StringDatabase.GetTable(TableName);
        int pivot = 1;
        for (int i = 0; i < Dialogues.Count; i++) {
            string message = sentences.GetEntry(TableName + "." + (i+pivot)).LocalizedValue;
            if (Dialogues[i].OptionCount > 0) {
                message = null;
                for(int j = 0; j < Dialogues[i].OptionCount; j++) {
                    string option = sentences.GetEntry(TableName + "." + (i+pivot)).LocalizedValue;
                    Dialogues[i].AddOption(option);
                    pivot += 1;
                }
                pivot -= 1;
            }

            Dialogues[i].Message = message;
        }
    }

    public void Converse() {
        GameManager.Instance.GetDialogueManager().StartDialogue(Dialogues);
    }
}
