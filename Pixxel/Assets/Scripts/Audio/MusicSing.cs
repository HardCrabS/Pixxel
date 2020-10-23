using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSing : MonoBehaviour 
{
    public static MusicSing Instance;

    AudioSource audio;
	void Start () 
    {
		if(FindObjectsOfType<MusicSing>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(this);
            Instance = this;
        }
        audio = GetComponent<AudioSource>();
        audio.volume = PlayerPrefsController.GetMasterVolume();
	}

    public void SetCurrentClip(AudioClip clip)
    {
        if(clip == audio.clip) { return; }

        audio.Stop();
        audio.clip = clip;
        audio.Play();
    }

    public void SetMusicVolume(float value)
    {
        audio.volume = value;
    }
}