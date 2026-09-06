using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/* The class that handles all the audio in the game. It has two sources for music. so they
 * can fade in and out of each other. The other sources are for the sound effects, the
 * dialogue, the footsteps and the UI. */
public class  AudioManager : MonoBehaviour {
    [Header("Mixer")]
    public AudioMixer Mixer; //This is the audio mixer that handles all the audio in the game.

    [Header("Music")]
    public AudioSource Music1; //The first music source.
    public AudioSource Music2; //The second music source.

    public List<AudioClip> MusicClips; //The list of all the music clips (the first one is
                                       //the combat music)

    public List<int> CurrentDefaultMusic; //The list of the background music in each area
                                          //of the scene. Each item corresponds to a clip from
                                          // MusicClips.

    public List<bool> CurrentMusicRepeats;

    public bool MusicRepeating; //Knows if the current music is repeating or not.
    public int CurrentMusicIndex = 0; //The current music clip of the CurrentDefaultMusic list.

    public float DefaultFadeTime = 2f; //The time it takes for the music to fade.

    bool Music1Playing = true; //Knows if the first audio source is playing.

    [Header("Steps")]
    PlayerMovementSounds PlayerSounds; //The script that handles the footsteps of the player.

    [Header("Sound Effects")]
    public List<AudioSource> Effects; //The list of sources that play the sound effects.
    int EffectIndex = 0; //The index of the next source to play a sound effect.

    public List<AudioClip> SwordHit; //The list of the sounds played when the player hits.
    public List<AudioClip> SwordHitAir; //The list of the sounds played when the player misses.
    public List<AudioClip> Deflect; //The list of sounds when deflecting an attack.

    [Header("Dialogue And Voice Lines")]
    public AudioSource DialogueSource; //The source that plays the voice lines.
    public AudioSource VoiceLines; //The source that plays the dialogue.

    [Header("UI Effects")]
    public AudioSource UIEffects; //The source that plays the UI sounds.
    public AudioClip UIClick; //The sound played when the player clicks a button.

    #region Unity Callbacks

    /* Here the audio sources are initialized and the mixer volume is set up according to the
     * values of the GameHandler. */
    private void Awake() {
        PlayerSounds = GetComponentInChildren<PlayerMovementSounds>();

        Music1.playOnAwake = false;
        Music2.playOnAwake = false;

        Music1Playing = true;
        Music1.volume = 1f;
        Music2.volume = 0f;

        foreach (AudioSource source in Effects) {
            source.playOnAwake = false;
            source.loop = false;
        }

        DialogueSource.playOnAwake = false;
        DialogueSource.loop = false;

        VoiceLines.playOnAwake = false;
        VoiceLines.loop = false;

        UIEffects.playOnAwake = false;
        UIEffects.loop = false;

        Mixer.SetFloat("Master", Mathf.Log10(GameHandler.MasterVolume) * 20);
        Mixer.SetFloat("Music", Mathf.Log10(GameHandler.MusicVolume) * 20);
        Mixer.SetFloat("Effects", Mathf.Log10(GameHandler.EffectsVolume) * 20);
        Mixer.SetFloat("Voice", Mathf.Log10(GameHandler.DialogueVolume) * 20);
        Mixer.SetFloat("UI", Mathf.Log10(GameHandler.UIVolume) * 20);
    }

    /* Here, after the Awake function and after the values have been set up, the music is set
     * to it's current clip and the loop acoordingly. */
    private void Start() {
        Music1.loop = MusicRepeating;
        Music2.loop = MusicRepeating;

        PlayMusic(GameHandler.CurrentMusicID);
    }

    /* Update is called once per frame. Here, a raycast is sent down from the player to check what surface
     * he is stepping on, so the game can play the appropriate sound. It also checks if the current music
     * repeats and if not, it checks if it is about to end, so it can play the next music in the list. */
    private void Update() {

        if (MusicRepeating) return;

        AudioSource source = Music2;
        if(Music1Playing) source = Music1;

        if(source.time < source.clip.length - DefaultFadeTime) return;

        CurrentMusicIndex += 1;
        MusicRepeating = CurrentMusicRepeats[CurrentMusicIndex];

        PlayMusic(CurrentMusicIndex);
    }
    #endregion

    #region Basic Audio Functions

    /* This function pauses all the audio sources in the game. */
    public void PauseAudio() {
        Music1.Pause();
        Music2.Pause();

        foreach (AudioSource source in Effects)
            source.Pause();

        DialogueSource.Pause();

        VoiceLines.Pause();

        UIEffects.Pause();

        foreach (SoundfulObject obj in FindObjectsByType<SoundfulObject>()) {
            obj.Pause();
        }
    }

    /* This function unpauses all the audio sources in the game. */
    public void UnPauseAudio() {
        Music1.UnPause();
        Music2.UnPause();

        foreach (AudioSource source in Effects)
            source.UnPause();

        DialogueSource.UnPause();

        VoiceLines.UnPause();

        UIEffects.UnPause();

        foreach (SoundfulObject obj in FindObjectsByType<SoundfulObject>()) {
            obj.UnPause();
        }
    }

    /* This function changes the volume of the audio mixer according to the source. */
    public void ChangeVolume(string source, float volume) {
        Mixer.SetFloat(source, Mathf.Log10(volume) * 20);
    }

    /* The default function to play a sound. It plays a Clip in the Source. */
    private void Sound(AudioSource Source, AudioClip Clip) {
        Source.clip = Clip;
        Source.Play();
    }
    #endregion

    #region Music Functions

    /* The function that plays music. It receives an id and a repeat value. If the id
     * is less than 0, it means the player enters combat and the combat music is played. If
     * the id is greater than the number of IDs in the Current default music, it resets to
     * the 0 value. The repeat value is used to know if the music should repeat or not.
     */
    public void PlayMusic(int id) {
        AudioSource source = Music1;

        if (Music1Playing)
            source = Music2;

        if (id < 0) {
            source.clip = MusicClips[0];
            MusicRepeating = true;

        } else if (id >= CurrentDefaultMusic.Count) {
            CurrentMusicIndex = 0;
            source.clip = MusicClips[CurrentDefaultMusic[0]];
            MusicRepeating = CurrentMusicRepeats[0];

        } else {
            CurrentMusicIndex = id;
            source.clip = MusicClips[CurrentDefaultMusic[id]];
            MusicRepeating = CurrentMusicRepeats[id];
        }

        if (!MusicRepeating) {
            GameHandler.CurrentMusicID = CurrentMusicIndex + 1;
            GameHandler.Save();
        }

        StopAllCoroutines();
        StartCoroutine(Fade(source));
        Music1Playing = !Music1Playing;
    }

    IEnumerator Fade(AudioSource SourceToGo) {
        AudioSource CurrentSource = Music1;
        if (SourceToGo == Music1)
            CurrentSource = Music2;

        float elapsedTime = 0f;
        SourceToGo.Play();
        while (SourceToGo.volume < 1) {
            CurrentSource.volume = Mathf.Lerp(1f, 0f, elapsedTime / DefaultFadeTime);
            SourceToGo.volume = Mathf.Lerp(0f, 1f, elapsedTime / DefaultFadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        CurrentSource.Stop();
    }
    #endregion


    #region Steps Functions

    /* This function plays the footsteps of the player. It checks what surface the player is stepping on
     * and plays a clip according to that surface. It also plays a sound of the player's clothes */
    public void PlayStep() {
        PlayerSounds.Step();
    }
    #endregion

    #region Sound Effects Functions

    /* The default function to play a sound effect. It plays a clip in the next available
     * source in the sources list. */
    public void PlayEffect(AudioClip Clip) {
        Sound(Effects[EffectIndex], Clip);
        EffectIndex++;
        if (EffectIndex >= Effects.Count)
            EffectIndex = 0;
    }


    /* This function plays a sound effect according to the id received. The id is used to
     * know which list of sounds to use. 0 is for sword hits, 1 is for sword hits in the air
     * and 2 is for deflects. */
    public void PlayCombatEffect(int id) {
        AudioClip Clip = SwordHit[UnityEngine.Random.Range(0, SwordHit.Count - 1)];

        if (id == 1)
            Clip = SwordHitAir[UnityEngine.Random.Range(0, SwordHitAir.Count - 1)];
        else if (id == 2)
            Clip = Deflect[UnityEngine.Random.Range(0, Deflect.Count)];

        PlayEffect(Clip);
        PlayerSounds.ClothSound();
    }

    #endregion

    #region Dialogues And Voice Lines

    /* This function plays a voice line whenever the player enters a VoiceLine territory */
    public void PlayVoiceLine(AudioClip clip) {
        Sound(VoiceLines, clip);
    }

    /* This function plays a dialogue whenever the DialogueManager goes to the next sentence. */
    public void PlayDialogue(AudioClip Clip) {
        Sound(DialogueSource, Clip);
    }
    #endregion

}