using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


/* Game Handler Class
 * The general class that handles the state of the game. Keeps the scene the player currently is,
 * if they are in the fantasy world or not, their position in each scene, their rotation, if they
 * have the input currently locked, if they are in combat, if they have unlocked the Shadow
 * Technique and a list of objects they have engaged with. This class also keeps track of the
 * volume of the music, effects, dialogue and UI buttons. This is the class that is first loaded
 * and saved during the game.
 */
public static class GameHandler
{
    public static int CurrentScene = 1; //Keeps track of the current scene
    public static bool FantasyMode = true; //Knows if the current scene is in the fantasy world or not

    public static Vector3 PlayerPosition = new Vector3(52, 13.2f, 118); //The player's positions. Defaulted in the first position in the first scene
    public static Vector3 PlayerRotation = new Vector3(0, -39, 0); //The player's rotation. Defaulted in the first position in the first scene

    public static bool LockedInput = false; //If the player has their input locked or not. Used for cutscenes and story events

    public static bool CombatLock = true; //Knows if the player is able to engage in combat or not
    public static bool ShadowLock = true; //Knows if the player has unlocked the Shadow Technique or not. Used to determine if the player can use the Shadow Technique

    public static List<string> EngagedObjects = new List<string>(); //A list with all the IDs of the object has engaged with. It is later used by the PlayerManager
                                                                    //to engage all the objects the player has already engaged with when they load a scene.

    public static int CurrentMusicID = 1; //The ID of the current music. Defaulted to 1 meaning the first music of the game;
    public static bool CurrentMusicRepeats = false; //If the current music repeats or not. Defaulted to false, meaning after this there will be another music playing

    public static float MasterVolume = 1f; //The master volume of the game. Defaulted to 1, but can be changed in the options menu and is saved and loaded with the game
    public static float MusicVolume = 1f; //The volume of the music. Defaulted to 1, but can be changed in the options menu and is saved and loaded with the game
    public static float EffectsVolume = 1f; //The volume of the effects. Defaulted to 1, but can be changed in the options menu and is saved and loaded with the game
    public static float DialogueVolume = 1f; //The volume of the dialogue. Defaulted to 1, but can be changed in the options menu and is saved and loaded with the game
    public static float UIVolume = 1f; //The volume of the UI buttons. Defaulted to 1, but can be changed in the options menu and is saved and loaded with the game

    /* The AddObject function adds the ID of an object to the list of engaged objects if it is
     * not already in the list. This is used to keep track of which objects the player has
     * engaged with, so that when they load a scene, they can engage with those objects again.
     */
    public static void AddObject(string objID) {
        if(objID == null || EngagedObjects.Contains(objID))
            return;

        EngagedObjects.Add(objID);
    }

    #region Saving And Loading

    /* Creates a SaveData object with the current state of the game and saves it using the Saver
     * class. This is called every time the player engages with an object, so that the game is
     * always saved after an interaction. It is also called in the options menu when the player
     * changes the volume, so that the volume settings are saved as well.
     */
    public static void Save() {
        Saver.SaveGame(new SaveData());
    }

    /* Loads the game using the Saver class and sets the state of the game to the loaded data. This
     * is called when the player loads a game, so that they can continue from where they left off.
     * if the data is null (meaning no save file was found), it does nothing and keeps the default
     * state of the game.
     */
    public static void Load() {
        SaveData data = Saver.LoadGame();

        if(data == null) return;

        CurrentScene = data.Scene;
        FantasyMode = data.FantasyMode;

        PlayerPosition = new Vector3(data.PlayerPositionX, data.PlayerPositionY, data.PlayerPositionZ);
        PlayerRotation = new Vector3(data.PlayerRotationX, data.PlayerRotationY, data.PlayerRotationZ);

        LockedInput = data.LockedInput;

        CombatLock = data.CombatLock;
        ShadowLock = data.ShadowLock;

        EngagedObjects = new List<string>(data.EngagedObjects);

        MasterVolume = data.MasterVolume;
        MusicVolume = data.MusicVolume;
        EffectsVolume = data.EffectsVolume;
        DialogueVolume = data.DialogueVolume;
        UIVolume = data.UIVolume;

        CurrentMusicID = data.CurrentMusicID;
        CurrentMusicRepeats = data.CurrentMusicRepeats;
    }

    #endregion
}


/* A serializable class that uses primitive types to store the state of the game. This is
 * the class that is saved and loaded using the Saver class. It has a constructor that takes
 * the current state of the game and sets its fields accordingly. The fields are all public,
 * so they can be easily accessed and modified when loading the game.
 */
[System.Serializable]
public class SaveData
{
    public int Scene;
    public bool FantasyMode;

    public float PlayerPositionX;
    public float PlayerPositionY;
    public float PlayerPositionZ;

    public float PlayerRotationX;
    public float PlayerRotationY;
    public float PlayerRotationZ;

    public bool LockedInput;

    public bool CombatLock;
    public bool ShadowLock;

    public string[] EngagedObjects;

    public float MasterVolume;
    public float MusicVolume;
    public float EffectsVolume;
    public float DialogueVolume;
    public float UIVolume;

    public int CurrentMusicID;
    public bool CurrentMusicRepeats;
    public SaveData() {
        Scene = GameHandler.CurrentScene;

        FantasyMode = GameHandler.FantasyMode;

        PlayerPositionX = GameHandler.PlayerPosition.x;
        PlayerPositionY = GameHandler.PlayerPosition.y;
        PlayerPositionZ = GameHandler.PlayerPosition.z;

        PlayerRotationX = GameHandler.PlayerRotation.x;
        PlayerRotationY = GameHandler.PlayerRotation.y;
        PlayerRotationZ = GameHandler.PlayerRotation.z;

        LockedInput = GameHandler.LockedInput;
        CombatLock = GameHandler.CombatLock;
        ShadowLock = GameHandler.ShadowLock;

        EngagedObjects = GameHandler.EngagedObjects.ToArray();

        MasterVolume = GameHandler.MasterVolume;
        MusicVolume = GameHandler.MusicVolume;
        EffectsVolume = GameHandler.EffectsVolume;
        DialogueVolume = GameHandler.DialogueVolume;
        UIVolume = GameHandler.UIVolume;

        CurrentMusicID = GameHandler.CurrentMusicID;
        CurrentMusicRepeats = GameHandler.CurrentMusicRepeats;
    }
}
