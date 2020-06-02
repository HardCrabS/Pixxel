using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] GameObject loadingPanel;
    [SerializeField] Slider slider;

    public void CallUnlockAllBoosts()
    {
        GameData.gameData.UnlockAllBoosts();  // JUST FOR TEST. REMOVE LATER
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

    public void LoadConcreteWorld(string world_name)
    {
            StartCoroutine(LoadAsynchronously(world_name));
    }

    IEnumerator LoadAsynchronously(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
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
