using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SoundfulObject : MonoBehaviour
{
    protected List<AudioSource> SoundSources;
    protected AudioSource DefaultSource;
    protected float Volume = .5f;

    protected virtual void SoundfulAwake() { }

    private void Awake() {
        SoundSources = SetSources();

        foreach (AudioSource source in SoundSources) {
            source.playOnAwake = false;
            source.loop = false;
            source.volume = Volume;
        }

        DefaultSource = SoundSources[0];

        SoundfulAwake();
    }

    protected virtual List<AudioSource> SetSources() {
        this.gameObject.AddComponent<AudioSource>();
        List<AudioSource> sources = new List<AudioSource>();
        sources.Add(GetComponent<AudioSource>());

        return sources;
    }

    public void Pause() {
        foreach (AudioSource source in SoundSources) {
            source.Pause();
        }
    }

    public void UnPause() {
        foreach (AudioSource source in SoundSources) {
            source.UnPause();
        }
    }

    protected void PlaySound(AudioClip clip) {
        if (DefaultSource == null) return;

        DefaultSource.clip = clip;
        DefaultSource.Play();
        FadeAudio(false, DefaultSource);
    }

    protected void PlaySound(AudioClip clip, int sourceIndex) {
        if (SoundSources[sourceIndex] == null) return;

        SoundSources[sourceIndex].clip = clip;
        SoundSources[sourceIndex].Play();
        FadeAudio(false, SoundSources[sourceIndex]);
    }

    protected void StopSound() {
        if (DefaultSource == null) return;

        FadeAudio(true, DefaultSource);
        DefaultSource.Stop();
    } 

    protected void StopSound(int sourceIndex) {
        if (SoundSources[sourceIndex] == null) return;

        FadeAudio(true, SoundSources[sourceIndex]);
        SoundSources[sourceIndex].Stop();
    }

    IEnumerator FadeAudio(bool FadeOut, AudioSource source) {
        float From = 0;
        float To = 1;

        if (FadeOut) {
            From = 1;
            To = 0;
        }

        float elapsedTime = 0f;
        while (source.volume != To) {
            source.volume = Mathf.Lerp(From, To, elapsedTime / 1);
            elapsedTime += Time.deltaTime;
            Debug.Log(source.volume);
            yield return null;
        }
    }
}
