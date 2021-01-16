using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour 
{
    public static AudioController Instance { get; private set; }

    AudioSource audioSource;
	void Awake () 
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefsController.GetMasterVolume();
	}
    public void PlayClip()
    {
        audioSource.Play();
    }
    public void SetCurrentClip(AudioClip clip)
    {
        if(clip == audioSource.clip) { return; }

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
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

    public void Pause()
    {
        audioSource.Pause();
    }
    public void Resume()
    {
        audioSource.Play();
    }
}