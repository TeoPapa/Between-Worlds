using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Music")]
    public AudioSource Music1;
    public AudioSource Music2;
    public float DefaultFadeTime = 2f;
    bool playingMusic1;

    List<AudioClip> MusicClips; //0: Combat, And after that the different area's loop music
    int MusicIndex = 1;

    [Header("Dialogue")]
    public AudioSource Dialogue1;
    public AudioSource Dialogue2;

    [Header("Steps")]
    public AudioSource Steps;

    public List<AudioClip> GrassSteps;
    public List<AudioClip> MudSteps;
    //public List<AudioClip> WoodSteps;
    //public List<AudioClip> StoneSteps;

    [Header("Sound Effects")]
    public List<AudioSource> Effects;
    int EffectIndex;

    public List<AudioClip> SwordHit;
    public List<AudioClip> SwordHitAir;
    public List<AudioClip> Deflect;

    [Header("Enemy Sound Effects")]
    public List<AudioSource> EnemyIdle;
    public int[] EnemyIDs;
    int EnemyIndex;
    public AudioSource EnemySteps;

    [Header("UI Effects")]
    public AudioSource UIEffects;

    [Header("What Is Ground")]
    public LayerMask Ground;

    [Header("Mixer")]
    public AudioMixer Mixer;

    string currentSurface;

    float currentMusicTime;


    #region Unity Callbacks
    private void Awake() {
        EnemyIndex = 0;
        currentMusicTime = 0f;

        playingMusic1 = true;
        Music1.volume = 1f;
        Music2.volume = 0f;

        Steps.playOnAwake = false;

        Music2.playOnAwake = false;

        foreach (AudioSource source in Effects) {
            source.playOnAwake = false;
            source.loop = false;
        }

        foreach (AudioSource source in EnemyIdle) {
            source.playOnAwake = false;
            source.loop = true;
        }

        EnemyIDs = new int[EnemyIdle.Count];

        UIEffects.playOnAwake = false;

        Dialogue1.playOnAwake = false;

        Dialogue2.playOnAwake = false;

        if(Music1.clip == null) {
            Music1.clip = MusicClips[1];
        }

        Mixer.SetFloat("Master", Mathf.Log10(GameHandler.MasterVolume) * 20);
        Mixer.SetFloat("Music", Mathf.Log10(GameHandler.MusicVolume) * 20);
        Mixer.SetFloat("Effects", Mathf.Log10(GameHandler.EffectsVolume) * 20);
        Mixer.SetFloat("Voice", Mathf.Log10(GameHandler.DialogueVolume) * 20);
        Mixer.SetFloat("UI", Mathf.Log10(GameHandler.UIVolume) * 20);
    }

    public void Start() {
        Steps.loop = false;

        Music2.loop = true;

        UIEffects.loop = false;
        Dialogue1.loop = false;
        Dialogue2.loop = false;
    }

    private void Update() {
        RaycastHit hit;

        Ray ray = new Ray(GameManager.Instance.GetMovement().gameObject.transform.position + Vector3.up * 0.1f, Vector3.down);

        if (Physics.Raycast(ray, out hit, 1f, Ground)) {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            if (renderer != null) {
                currentSurface = renderer.material.name;
            }
        }

        AudioSource source = Music1;
        if(!playingMusic1)
            source = Music2;
        currentMusicTime += Time.deltaTime;
        if (source.clip != MusicClips[0]) {
            if (currentMusicTime >= source.clip.length - DefaultFadeTime) {
                MusicChange(MusicIndex);
            }
        }
    }

    #endregion

    #region Setters and Getters

    public void SetIDAndStart(int id, bool repeat) {
        MusicIndex = id;
        Music1.clip = MusicClips[id];

        if (repeat) return;

        MusicIndex++;
    }

    public void SetMusicClips(List<AudioClip> mcl) {
        MusicClips = mcl;
    }

    public void PlayMusic(int id, bool skip) {
        if (id == 0) {
            MusicChange(id);
            return;
        }
        else if(id < MusicClips.Count)
            MusicIndex = id;
        else
            MusicIndex = 1;

        MusicChange(id);

        if (!skip) return;

        MusicIndex++;

        if(MusicIndex >= MusicClips.Count) {
            MusicIndex = 1;
        }
    }

    #endregion

    #region Music
    public void StopMusic() {
        MusicChange(MusicIndex);
    }

    void MusicChange(int id) {
        AudioSource source1 = Music1;
        AudioSource source2 = Music2;

        if(!playingMusic1) {
            source1 = Music2;
            source2 = Music1;
        }

        if (source1.clip == MusicClips[id])
            return;

        source2.clip = MusicClips[id];
        StopAllCoroutines();
        StartCoroutine(Fade(source1, source2, DefaultFadeTime));

        playingMusic1 = !playingMusic1;
    }

    IEnumerator Fade(AudioSource source1, AudioSource source2, float fadeTime) {
        float elapsedTime = 0f;
        source2.Play();
        currentMusicTime = 0;
        while (source2.volume < 1) {
            source1.volume = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
            source2.volume = Mathf.Lerp(0f, 1f, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        source1.Stop();
    }

    IEnumerator Fade(AudioSource source, float fadeTime) {
        float elapsedTime = 0f;

        source.Play();
        currentMusicTime = 0;
        while (elapsedTime < fadeTime) {
            source.volume = Mathf.Lerp(0f, 1f, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    #endregion

    #region Sound Effects
    public void PlayStep() {
        List<AudioClip> surfaceSteps = null;

        switch(currentSurface) {
            case "Mud":
                surfaceSteps = MudSteps;
                break;
            /*
            case string a when a.Contains("Wood"):
                surfaceSteps = WoodSteps;
               break;
            case string a when a.Contains("Stone"):
                surfaceSteps = StoneSteps;
               break;
            */
            default:
                surfaceSteps = GrassSteps;
                break;

        }

        Sound(Steps, surfaceSteps[Random.Range(0, surfaceSteps.Count - 1)]);
    }

    public void PlayEnemySound(AudioClip Clip, int ID) {
        EnemyIDs[EnemyIndex] = ID;

        EnemyIdle[EnemyIndex].clip = Clip;
        EnemyIdle[EnemyIndex].Play();

        EnemyIndex++;

        if (EnemyIndex >= EnemyIdle.Count) {
            EnemyIndex = 0;
        }
    }

    public void PlayEnemySteps(AudioClip Clip) {
        Sound(EnemySteps, Clip);
    }

    public void StopEnemySound(int ID) {
        int index = System.Array.IndexOf(EnemyIDs, ID);
        if(index == -1) return;

        EnemyIdle[index].Stop();
        EnemyIndex = index;
    }

    public void PlayEffect(AudioClip Clip) {
        Sound(Effects[EffectIndex], Clip);
        EffectIndex++;
        if (EffectIndex >= Effects.Count) {
            EffectIndex = 0;
        }
    }

    public void PlayEffect(int id) {
        AudioClip Clip = SwordHit[Random.Range(0, SwordHit.Count - 1)];

        if (id == 1)
            Clip = SwordHitAir[Random.Range(0, SwordHitAir.Count - 1)];
        else if (id == 2)
            Clip = Deflect[Random.Range(0, Deflect.Count)];

        PlayEffect(Clip);
    }

    #endregion

    #region Dialogues And Voice Lines
    public void PlayDialogue(AudioClip Clip) {
        Sound(Dialogue2, Clip);
    }

    public void PlayVoiceLine(AudioClip clip) {
        Sound(Dialogue1, clip);
    }
    #endregion

    public void PauseAudio() {
        Music1.Pause();
        Music2.Pause();

        Dialogue1.Pause();
        Dialogue2.Pause();

        Steps.Pause();

        EnemySteps.Pause();

        foreach (AudioSource source in Effects)
            source.Pause();

        foreach (AudioSource source in EnemyIdle)
            source.Pause();
    }

    public void UnPauseAudio() {
        Music1.UnPause();
        Music2.UnPause();

        Dialogue1.UnPause();
        Dialogue2.UnPause();

        Steps.UnPause();

        EnemySteps.UnPause();

        foreach (AudioSource source in Effects)
            source.UnPause();

        foreach (AudioSource source in EnemyIdle)
            source.UnPause();
    }

    public void ChangeVolume(string source, float volume) {
        Mixer.SetFloat(source, Mathf.Log10(volume) * 20);
    }

    private void Sound(AudioSource Source, AudioClip Clip) {
        Source.clip = Clip;
        Source.Play();
    }

}