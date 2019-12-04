using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
	public void LoadNextScene()
    {
        int currScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currScene + 1);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ReloadLevel()
    {
        int currScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currScene);
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
