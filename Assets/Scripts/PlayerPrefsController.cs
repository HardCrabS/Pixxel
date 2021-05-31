using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsController : MonoBehaviour 
{
    const string MASTER_VOLUME_KEY = "VOLUME";
    const string MASTER_SFX_VOLUME_KEY = "SFX_VOLUME";
    const string MASTER_VISUALIZER = "VISUALIZER";

    public static void SetMasterVolume(float volume)
    {
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, volume);
    }

    public static float GetMasterVolume()
    {
        return PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 0.7f);
    }

    public static void SetMasterSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat(MASTER_SFX_VOLUME_KEY, volume);
    }
    public static float GetMasterSFXVolume()
    {
        return PlayerPrefs.GetFloat(MASTER_SFX_VOLUME_KEY, 1f);
    }
    public static void SetMasterVisualizer(int isActive)
    {
        PlayerPrefs.SetInt(MASTER_VISUALIZER, isActive);
    }
    public static int GetMasterVisualizer()
    {
        return PlayerPrefs.GetInt(MASTER_VISUALIZER, 1);
    }
}
