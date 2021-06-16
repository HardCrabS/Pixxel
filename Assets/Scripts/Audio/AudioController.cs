using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance { get; private set; }

    public delegate void OnSFXVolumeChange(float volume);
    public event OnSFXVolumeChange onSFXVolumeChange;

    public float SFXVolume { private set; get; }

    AudioSource audioSource;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);

        audioSource = GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefsController.GetMasterVolume();
        SFXVolume = PlayerPrefsController.GetMasterSFXVolume();
    }
    public void NotifyForVolumeChange()
    {
        float sfxVolumeMultiplier = PlayerPrefsController.GetMasterSFXVolume();
        onSFXVolumeChange?.Invoke(sfxVolumeMultiplier);
    }
    public void PlayClipOneShot(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    public void SetCurrentClip(AudioClip clip, float delay = 0)
    {
        if (clip == audioSource.clip && audioSource.isPlaying) { return; }

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.PlayDelayed(delay);
    }

    public void SetMusicVolume(float value)
    {
        audioSource.volume = value;
    }

    public AudioSource PlayNewClip(AudioClip clip, float volume = 1, Vector3 pos = new Vector3(), bool looping = false)
    {
        GameObject tempGO = new GameObject("Audio_" + clip.name);
        tempGO.transform.position = pos;

        float sfxVolumeMultiplier = PlayerPrefsController.GetMasterSFXVolume();
        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = clip;
        aSource.volume = volume * sfxVolumeMultiplier;

        aSource.Play();
        if (looping)
        {
            aSource.loop = true;
        }
        else
        {
            Destroy(tempGO, clip.length);
        }
        return aSource;
    }
    public void StartFade(float duration, float targetVolume)
    {
        StartCoroutine(StartFadeCo(duration, targetVolume));
    }
    IEnumerator StartFadeCo(float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }
    public void Pause()
    {
        audioSource.Pause();
    }
    public void Resume()
    {
        audioSource.Play();
    }
}