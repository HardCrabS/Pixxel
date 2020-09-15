﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] GameObject loadingPanel;
    [SerializeField] Slider slider;
    [SerializeField] AudioClip mainMenuSong;

    public void CallUnlockAllBoosts()
    {
        GameData.gameData.UnlockAllBoosts();  // JUST FOR TEST. TODO REMOVE LATER
    }
    public void LoadNextScene()
    {
        int currScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currScene + 1);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadConcreteWorld(string world_name, AudioClip worldSong)
    {
        StartCoroutine(LoadAsynchronously(world_name, worldSong));
    }

    IEnumerator LoadAsynchronously(string sceneName, AudioClip worldSong)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.completed += (asyncOperation) =>
        {
            MusicSing.Instance.SetCurrentClip(worldSong);
        };
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                slider.value = progress;

                yield return null;
            }
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        if (GameData.gameData != null)
            GameData.gameData.Save();
        SceneManager.LoadScene("Start");
        MusicSing.Instance.SetCurrentClip(mainMenuSong);
        Time.timeScale = 1;
    }

    public void LoadOptionsScene()
    {
        SceneManager.LoadScene("Options");
    }

    public void LoadBoostScene()
    {
        SceneManager.LoadScene("Boosts");
    }
}