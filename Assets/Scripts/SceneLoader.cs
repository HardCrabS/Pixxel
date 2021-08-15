using System.Collections;
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
        GameData.gameData.UnlockAllBoosts();  // FOR TEST.
    }
    public void CallUnlockAllWorlds()
    {
        GameData.gameData.UnlockAllWorlds();  // FOR TEST.
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

    public void LoadConcreteWorld(string world_name, AudioClip worldSong, float delay)
    {
        StartCoroutine(LoadAsynchronously(world_name, worldSong, delay));
    }
    public void LoadSceneAsync(int index) //used in splash scene
    {
        /*if (PlayerPrefs.GetInt("TUTORIAL", 0) == 0)
        {
            SceneManager.LoadSceneAsync("World");
        }
        else
        {*/
            AsyncOperation operation = SceneManager.LoadSceneAsync(index);
            operation.completed += (asyncOperation) =>
            {
                AudioController.Instance.SetCurrentClip(mainMenuSong);
            };
        //}
    }

    IEnumerator LoadAsynchronously(string sceneName, AudioClip worldSong, float delay)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.completed += (asyncOperation) =>
        {
            AudioController.Instance.SetCurrentClip(worldSong, delay);
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
            GameData.Save();
        SceneManager.LoadScene("Start");
        AudioController.Instance.SetCurrentClip(mainMenuSong);
        Time.timeScale = 1;
    }

    public void LoadWorldSelectScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("World Select");
        operation.completed += (asyncOperation) =>
        {
            AudioController.Instance.SetCurrentClip(mainMenuSong);
        };
    }
    public void PlaySpashSceneSound() //play sound in the splash scene
    {
        GetComponent<AudioSource>().Play();
    }
}