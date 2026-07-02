using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueType
{
    public string Speaker;
    public string Message;

    public AudioClip VoiceLine;

    public int OptionCount;
    private List<string> Options = new List<string>();
    public List<int> Jumps; 

    public void AddOption(string option)
    {
        Options.Add(option);
    }

    public AudioClip GetVoiceLine()
    {
        return VoiceLine;
    }

    public string GetSpeaker()
    {
        if (Speaker == null || Speaker == "*") return "Noathyn";

        if (Speaker == "**") return "";

        return Speaker;
    }

    public string GetMessage()
    {
        Message.Replace("*", "Noathyn");
        Message.Replace("**", "*");
        return Message;
    }

    public string GetOption(int x)
    {
        if(OptionCount <= 0) return null;

        return Options[x];
    }
}
