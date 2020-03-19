using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSpectrum : MonoBehaviour {
    private float[] audioSpectrum;
    public static float spectrumValue { get; private set; }

	void Start () {
        audioSpectrum = new float[128];
	}
	
	void Update () {
        AudioListener.GetSpectrumData(audioSpectrum, 0, FFTWindow.Hamming);
        if (audioSpectrum != null && audioSpectrum.Length >0)
        {
            spectrumValue = audioSpectrum[0] * 100;
        }
	}
}
