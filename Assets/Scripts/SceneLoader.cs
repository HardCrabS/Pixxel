using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] GameObject loadingCanvas;
    [SerializeField] AudioClip mainMenuSong;

    Slider slider;

    public void CallUnlockAllBoosts()
    {
        GameData.gameData.UnlockAllBoosts();  // FOR TEST.
    }
    public void CallUnlockAllWorlds()
    {
        GameData.gameData.UnlockAllWorlds();  // FOR TEST.
    }
    public void CallUnlockAllTrinkets()
    {
        GameData.gameData.UnlockAllTrinkets();  // FOR TEST.
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

    public void LoadConcreteWorld(string world_name, AudioClip worldSong, float delay, bool useLoadingPanel = false)
    {
        StartCoroutine(LoadAsynchronously(world_name, worldSong, delay, useLoadingPanel));
    }
    public void LoadSceneAsync(int index) //used in splash scene
    {
        if (PlayerPrefs.GetInt("WORLD TUTORIAL", 0) == 0)
        {
            SceneManager.LoadSceneAsync("World");
        }
        else
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(index);
            operation.completed += (asyncOperation) =>
            {
                AudioController.Instance.SetCurrentClip(mainMenuSong);
            };
        }
    }

    IEnumerator LoadAsynchronously(string sceneName, AudioClip worldSong, float delay, bool useLoadingPanel)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.completed += (asyncOperation) =>
        {
            AudioController.Instance.SetCurrentClip(worldSong, delay);
        };
        if (useLoadingPanel)
        {
            SpawnLoadingCanvas();

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
        AsyncOperation operation = SceneManager.LoadSceneAsync("Start");
        operation.completed += (asyncOperation) =>
        {
            AudioController.Instance.SetCurrentClip(mainMenuSong);
            Time.timeScale = 1;
        };
    }
    public void LoadWorldSelectScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("World Select");
        operation.completed += (asyncOperation) =>
        {
            AudioController.Instance.SetCurrentClip(mainMenuSong);
        };
    }
    public void LoadSceneAsync(string sceneName, bool useLoadingPanel = false)
    {
        StartCoroutine(LoadScene(sceneName, useLoadingPanel));
    }
    IEnumerator LoadScene(string sceneName, bool useLoadingPanel = false)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        if (useLoadingPanel)
        {
            SpawnLoadingCanvas();

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                slider.value = progress;

                yield return null;
            }
        }
    }
    IEnumerator LoadSceneWithAdCo(string sceneName)
    {
        if (GameData.gameData.saveData.adsRemoved || GridA.Instance.playTutorial)
        {
            StartCoroutine(LoadAsynchronously(sceneName, mainMenuSong, 0, true));
            yield break;
        }
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;//stop scene loading at 90%
        operation.completed += (asyncOperation) =>
        {
            AudioController.Instance.SetCurrentClip(mainMenuSong);
        };
        SpawnLoadingCanvas();

        VideoAd videoAd = new VideoAd();//video ad object
        videoAd.LoadAd();//load ad
        yield return new WaitUntil(() => videoAd.AdLoadingCompleted());//wait until ad is loaded
        videoAd.ShowAd();//show ad

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            slider.value = progress;

            if (videoAd.IsFinished())//ad finished playing
            {
                operation.allowSceneActivation = true;//continue loading next scene
            }
            yield return null;
        }
    }
    public void LoadSceneWithAd(string sceneName)
    {
        StartCoroutine(LoadSceneWithAdCo(sceneName));
    }
    public void PlaySpashSceneSound() //play sound in the splash scene
    {
        GetComponent<AudioSource>().Play();
    }
    void SpawnLoadingCanvas()
    {
        GameObject canvas = Instantiate(loadingCanvas);
        slider = canvas.GetComponentInChildren<Slider>();
    }
}