using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    [Header("World tutorial")]
    [SerializeField] GameObject mainGameCanvas;
    [SerializeField] GameObject tutorialCanvasPrefab;

    [SerializeField] Transform crosshair;
    [SerializeField] Glitcher glitcher;
    [SerializeField] GameObject backgroundPrefab;
    [SerializeField] WorldInformation currentWorldInfo;
    [SerializeField] AudioClip intenceMusic;

    [Header("World select tutorial")]
    [SerializeField] UI_System worldSelectUISystem;
    [SerializeField] UI_Screen selectEmptyScreen;
    [SerializeField] UI_Screen selectMainScreen;

    [Header("Dialogues")]
    [SerializeField] Dialogue login;
    [SerializeField] Dialogue hello;
    [SerializeField] Dialogue blocksShow;
    [SerializeField] Dialogue blocksMatched;
    [SerializeField] Dialogue moreBlocks;
    [SerializeField] Dialogue worldSelect;

    UI_System uiSystem;
    UI_Screen[] screens;//0 - PAM, 1 - login, 2 - empty

    SequentialText text;
    AudioSource audioSource;
    string username;

    GameObject tutorialCanvas;
    GameObject background;

    const string WORLD_TUTORIAL = "WORLD TUTORIAL";
    const string WORLD_SELECT_TUTORIAL = "WORLD SELECT TUTORIAL";

    void Awake()
    {
        bool worldTutorCompleted = PlayerPrefs.GetInt(WORLD_TUTORIAL, 0) == 1;
        bool worldSelectTutorColmpleted = PlayerPrefs.GetInt(WORLD_SELECT_TUTORIAL, 0) == 1;

        if (!worldTutorCompleted && SceneManager.GetActiveScene().name == "World")
        {
            WorldTutorial();
            mainGameCanvas.SetActive(false);
            GridA.Instance.playTutorial = true;
        }
        else if (!worldSelectTutorColmpleted && SceneManager.GetActiveScene().name == "World Select")
        {
            if (GameData.gameData)
                username = GameData.gameData.saveData.playerInfo.username;
            WorldSelectTutorial();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void WorldTutorial()
    {
        SpawnWorldTutorialGraphics();
        SpawnSpaceBackground();
        StartCoroutine(PlayWorldTutorial());
    }
    void WorldSelectTutorial()
    {
        SpawnWorldTutorialGraphics();
        StartCoroutine(PlayWorldSelectTutorial());
    }

    void SpawnWorldTutorialGraphics()
    {
        tutorialCanvas = Instantiate(tutorialCanvasPrefab);
        uiSystem = tutorialCanvas.GetComponent<UI_System>();
        screens = tutorialCanvas.GetComponentsInChildren<UI_Screen>();
        text = tutorialCanvas.GetComponentInChildren<SequentialText>();
    }

    void SpawnSpaceBackground()
    {
        background = Instantiate(backgroundPrefab);
        var canvas = background.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
        canvas.sortingOrder = 1;
    }

    IEnumerator PlayWorldTutorial()
    {
        yield return new WaitForSeconds(1f);
        uiSystem.SwitchScreens(screens[0]);
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(PlayDialogue(login));//ask user to log in

        uiSystem.SwitchScreens(screens[1]);//screen to enter username
        screens[1].GetComponent<UsernameCheck>().onUsernameConfirm
            .AddListener(() =>
            {
                uiSystem.SwitchScreens(screens[0]);
                username = screens[1].GetComponent<UsernameCheck>().inputField.text;
            });//open PAM after username is confirmed

        yield return new WaitUntil(() => uiSystem.CurrentScreen == screens[0]);//wait until PAM shows

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(PlayDialogue(hello));//greet player speech

        uiSystem.SwitchScreens(screens[2]);//switch to empty screen so PAM fades out
        yield return new WaitForSeconds(0.5f);

        background.GetComponent<CanvasGroup>().DOFade(0, 1);//fade out background
        glitcher.GetComponent<AudioSource>().Play();
        yield return StartCoroutine(glitcher.Glitching(false));//glitch & scroll fx

        uiSystem.SwitchScreens(screens[0]);//switch to PAM screen
        yield return new WaitForSeconds(1);//wait while PAM appears

        yield return StartCoroutine(BlocksMatchTutorial());//blocks match tutorial

        glitcher.SetBombExplodeParameters();
        yield return StartCoroutine(MoreBlocksFallIn());//more blocks

        yield return new WaitForSeconds(2);
        Destroy(tutorialCanvas);
        PlayerPrefs.SetInt(WORLD_TUTORIAL, 1);

        yield return new WaitUntil(() => LevelSlider.Instance.GetGameLevel() == 1);
        GridA.Instance.IncreaseBombSpawnChance(50);
    }

    IEnumerator BlocksMatchTutorial()
    {
        GridA.Instance.FillTutorialLayout();//spawn blocks
        yield return StartCoroutine(PlayDialogue(blocksShow));//show blocks speech

        GridA.Instance.currState = GameState.move;//allow moving blocks
        var movingCrosshair = StartCoroutine(MoveCrosshair());//crosshair shows which blocks to move
        yield return new WaitWhile(() => GridA.Instance.allBoxes[2, 7] != null);//wait while correct block isn't destroyed
        StopCoroutine(movingCrosshair);//stop moving crosshair
        crosshair.position = new Vector2(-10, 0);//hide crosshair

        yield return StartCoroutine(PlayDialogue(blocksMatched));//blocks matched speech
    }

    IEnumerator MoreBlocksFallIn()
    {
        yield return new WaitForSeconds(1f);
        Camera.main.GetComponent<CameraShake>().ShakeCam(2, 1);
        yield return new WaitForSeconds(2f);//wait while camera is shaking

        GridA.Instance.SetDefaultTemplate();//spawn default board 8x8

        audioSource.clip = intenceMusic;//play intence music
        audioSource.Play();

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(PlayDialogue(moreBlocks));//more blocks speech

        yield return audioSource.DOFade(0, 0.5f).WaitForCompletion();//fade out intence music
        AudioController.Instance.SetCurrentClip(currentWorldInfo.Song, fadeIn: true);//fade in world clip music

        mainGameCanvas.SetActive(true);
        uiSystem.SwitchScreens(screens[2]);//PAM goes away
    }

    IEnumerator PlayWorldSelectTutorial()
    {
        worldSelectUISystem.m_StartScreen = selectEmptyScreen;//set empty screen on start

        yield return new WaitForSeconds(1f);
        uiSystem.SwitchScreens(screens[0]);//PAM
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(PlayDialogue(worldSelect));//world select speech
        yield return new WaitForSeconds(0.5f);
        uiSystem.SwitchScreens(screens[2]);//PAM goes away
        yield return new WaitForSeconds(1f);

        worldSelectUISystem.SwitchScreens(selectMainScreen);//main screen with worlds
        PlayerPrefs.SetInt(WORLD_SELECT_TUTORIAL, 1);
        yield return new WaitForSeconds(1f);
        Destroy(tutorialCanvas);
    }
    IEnumerator PlayDialogue(Dialogue dialogue)
    {
        foreach (var dialogueText in dialogue.Text)
        {
            string message = dialogueText;
            if (dialogueText.Contains("[username]"))
            {
                message = dialogueText.Replace("[username]", "[" + username + "]");
            }
            text.Clear();
            yield return new WaitForSeconds(0.15f);
            text.PlayMessage(message);
            yield return new WaitWhile(() => text.PlayingMessage);
            yield return new WaitForSeconds(0.25f);
        }
    }

    IEnumerator MoveCrosshair()
    {
        float speed = 2;
        Vector2 start = GridA.Instance.allBoxes[3, 7].transform.position;
        Vector2 end = GridA.Instance.allBoxes[3, 6].transform.position;

        Vector2 target = end;
        crosshair.position = start;

        while (true)
        {
            float step = speed * Time.deltaTime;
            crosshair.position = Vector2.MoveTowards(crosshair.position, target, step);

            if (Vector3.Distance(crosshair.position, target) < 0.001f)
            {
                target = target == end ? start : end;
            }
            yield return null;
        }
    }
}