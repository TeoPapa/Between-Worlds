using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Assemblies;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [HideInInspector]
    public static GameManager Instance;

    public int CurrentSceneIndex = 0;

    public GameObject PlayerPrefab;

    public bool ReplayFirstClip = false; //If the first song on the clips will be looped

    [HideInInspector]
    public GameObject Player;

    AudioManager SoundManager;
    DialogueManager Dialogue;
    VoiceLineManager VoiceLines;
    UIManager UserInterface;

    GameObject FirstPersonCamera;
    GameObject ThirdPersonCamera;

    PlayerMovement PlayerMoving;
    PlayerCombat PlayerCombatting;
    PlayerInteractions PlayerInteracting;

    bool isPaused = false;
    private void Awake() {
        Instance = this;

        GameHandler.Load();

        if(SceneManager.GetActiveScene().buildIndex == 0)
            return;

        PlayerMovement pl = FindAnyObjectByType<PlayerMovement>();

        if (pl != null) {
            Destroy(pl.gameObject.transform.parent.gameObject);
        }

        GameObject NewPlayer = Instantiate(PlayerPrefab, GameHandler.PlayerPosition, Quaternion.Euler(GameHandler.PlayerRotation));

        Player = NewPlayer.GetComponentInChildren<PlayerMovement>().gameObject;

        SoundManager = this.GetComponentInChildren<AudioManager>();
        Dialogue = this.GetComponentInChildren<DialogueManager>();
        VoiceLines = this.GetComponentInChildren<VoiceLineManager>();
        UserInterface = this.GetComponentInChildren<UIManager>();

        PlayerMoving = Player.GetComponent<PlayerMovement>();
        PlayerCombatting = Player.GetComponent<PlayerCombat>();
        PlayerInteracting = Player.GetComponent<PlayerInteractions>();

        bool Fantasy = GameHandler.FantasyMode;

        if (!Fantasy) {
            PlayerCombatting.CombatLock = true;
            PlayerCombatting.ShadowLock = true;
        }

        FirstPersonCamera = NewPlayer.GetComponentInChildren<CinemachineHardLockToTarget>().gameObject;
        ThirdPersonCamera = NewPlayer.GetComponentInChildren<CinemachineOrbitalFollow>().gameObject;

        PlayerMoving.CurrentCamera = CameraSwitch(Fantasy);

    }

    public void ChangeMusicId(int id) {
        GameHandler.CurrentMusicID = id;
        SoundManager.PlayMusic(id);
        Save();
    }

    private void Start() {
        List<ObjectID> objects = new List<ObjectID>(FindObjectsByType<ObjectID>());
        string[] EngagedObjects = GameHandler.EngagedObjects.ToArray();

        for (int i = 0; i < EngagedObjects.Length; i++  ) {
            string ID = EngagedObjects[i];

            ObjectID obj = objects.Find(x => x.GetID() == ID);
            if (obj != null) {
                obj.Engaged();
            }
        }
    }

    public PlayerMovement GetMovement() {
        return PlayerMoving;
    }

    public PlayerCombat GetCombat() {
        return PlayerCombatting;
    }

    public PlayerInteractions GetInteractions() {
        return PlayerInteracting;
    }

    public AudioManager GetAudioManager() {
        return SoundManager;
    }

    public DialogueManager GetDialogueManager() {
        return Dialogue;
    }

    public VoiceLineManager GetVoiceLineManager() {
        return VoiceLines;
    }

    public UIManager GetUIManager() {
        return UserInterface;
    }

    public void GoToCombat() {
        SoundManager.PlayMusic(-1);
    }

    public void ReturnFromCombat() {
        int id = GameHandler.CurrentMusicID;

        SoundManager.PlayMusic(id);
    }

    public void Save() {
        GameHandler.PlayerPosition = Player.transform.position;
        GameHandler.PlayerRotation = Player.transform.rotation.eulerAngles;

        GameHandler.CombatLock = PlayerCombatting.CombatLock;
        GameHandler.ShadowLock = PlayerCombatting.ShadowLock;

        GameHandler.LockedInput = PlayerMoving.isLocked();

        GameHandler.Save();
    }

    public void ChangeScene(int scene, bool fantasy, Vector3 pos, Vector3 rot) {
        //TODO: Check if the player changes worlds in order to play the animation
        bool CurrentMode = GameHandler.FantasyMode;
        GameHandler.CurrentScene = scene;
        GameHandler.PlayerPosition = pos;
        GameHandler.PlayerRotation = rot;
        GameHandler.FantasyMode = fantasy;

        StartCoroutine(LoadSceneAsync(scene, CurrentMode));
    }

    GameObject CameraSwitch(bool mode) {
            ThirdPersonCamera.SetActive(mode);
            FirstPersonCamera.SetActive(!mode);

        if (mode) return ThirdPersonCamera;

        return FirstPersonCamera;
    }

    IEnumerator LoadSceneAsync(int scene, bool curMode) {
        PlayerMoving.LockInput();

        if(curMode != GameHandler.FantasyMode && curMode) {
            PlayerMoving.CurrentCamera = CameraSwitch(GameHandler.FantasyMode);
        }


        UserInterface.FadeIn();

        yield return new WaitForSeconds(3f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone) {
            if(asyncLoad.progress >= 0.9f) {
                UserInterface.FadeOut();
                PlayerMoving.UnlockInput();
                asyncLoad.allowSceneActivation = true;
                Save();
            }

            yield return null;
        }
    }

    public void PlayerDeath() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
