using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSing : MonoBehaviour {
    AudioSource audio;
	void Start () {
		if(FindObjectsOfType<MusicSing>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(this);
        }
        audio = GetComponent<AudioSource>();
	}
	// Update is called once per frame
	void Update () {
        audio.volume = PlayerPrefsController.GetMasterVolume();
	}
}
