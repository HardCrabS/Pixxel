using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;
    [SerializeField] GameObject pausePanel;

    ScrollBackground scrollBackground;

    void Start()
    {
        if (volumeSlider != null)
        {
            volumeSlider.value = PlayerPrefsController.GetMasterVolume();
            volumeSlider.onValueChanged.AddListener(delegate { MusicSing.Instance.SetMusicVolume(volumeSlider.value); });
        }
        else
        {
            scrollBackground = FindObjectOfType<ScrollBackground>();
        }
    }

    public void Pause()
    {
        pausePanel.SetActive(true);
        ScrollBackground.Instance.StopScrolling();
        Time.timeScale = 0;
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        ScrollBackground.Instance.ResumeScrolling();
        Time.timeScale = 1;
    }

    public void SaveVolume()
    {
        PlayerPrefsController.SetMasterVolume(volumeSlider.value);
    }
}
