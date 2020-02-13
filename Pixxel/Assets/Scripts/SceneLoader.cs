﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour {
    [SerializeField] GameObject loadingPanel;
    [SerializeField] Slider slider;
	public void LoadNextScene()
    {
        int currScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currScene + 1);
    }

    public void LoadConcreteWorld(string world_name)
    {
        StartCoroutine(LoadAsynchronously(world_name));
    }

    IEnumerator LoadAsynchronously(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        loadingPanel.SetActive(true);
        
        while(!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            slider.value = progress;

            yield return null;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        Level level = FindObjectOfType<Level>();
        if (level != null)
            level.SaveProgress();
        SceneManager.LoadScene("Start");
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
