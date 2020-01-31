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

    public void LoadConcreteWorld(string w_number)
    {
        SceneManager.LoadScene(w_number);
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
