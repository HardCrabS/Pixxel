using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider SFXSlider;
    [SerializeField] Toggle visualizerToggle;
    [SerializeField] GameObject visualizerCanvas;

    void Start()
    {
        volumeSlider.value = PlayerPrefsController.GetMasterVolume();
        SFXSlider.value = PlayerPrefsController.GetMasterSFXVolume();
        if (visualizerToggle != null)
            visualizerToggle.isOn = System.Convert.ToBoolean(PlayerPrefsController.GetMasterVisualizer());
        visualizerToggle.onValueChanged.AddListener(delegate
        {
            if (visualizerCanvas != null)
                visualizerCanvas.SetActive(visualizerToggle.isOn);
        });
        volumeSlider.onValueChanged.AddListener(delegate { AudioController.Instance.SetMusicVolume(volumeSlider.value); });
    }

    public void Pause()
    {
        AudioController.Instance.Pause();
        ScrollBackground.Instance.StopScrolling();
    }

    public void Resume()
    {
        AudioController.Instance.Resume();
        ScrollBackground.Instance.ResumeScrolling();
    }

    public void SaveSettings()
    {
        PlayerPrefsController.SetMasterVolume(volumeSlider.value);
        PlayerPrefsController.SetMasterSFXVolume(SFXSlider.value);
        if (visualizerToggle != null)
            PlayerPrefsController.SetMasterVisualizer(System.Convert.ToInt32(visualizerToggle.isOn));
    }
}