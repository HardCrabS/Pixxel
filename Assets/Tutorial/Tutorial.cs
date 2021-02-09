using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] UI_System uiSystem;
    [SerializeField] GameObject visualizerCanvas;
    [SerializeField] GameObject tutorialCanvas;
    [SerializeField] GameObject logo;
    [SerializeField] SequentialText text;
    [SerializeField] Dialogue[] helloDialogues;
    [SerializeField] Dialogue[] blocksDialogues;
    [SerializeField] Dialogue[] finalDialogues;

    [SerializeField] Transform crosshair;
    [SerializeField] Glitcher glitcher;
    [SerializeField] GameObject background;
    [SerializeField] AudioClip worldClip;

    UI_Screen gameUI;

    void Awake()
    {
        if (PlayerPrefs.GetInt("TUTORIAL", 0) == 0)
        {
            logo.SetActive(false);
            visualizerCanvas.SetActive(false);
            gameUI = uiSystem.m_StartScreen;
            uiSystem.m_StartScreen = tutorialCanvas.GetComponent<UI_Screen>();
            //uiSystem.m_Fader = fadeImage;
            //gameCanvas.SetActive(false);
            tutorialCanvas.SetActive(true);
            StartCoroutine(PlayTutorial());
        }
        else
        {
            glitcher.SetBombExplodeParameters();
            Destroy(tutorialCanvas);
            Destroy(gameObject);
        }
    }

    public IEnumerator PlayTutorial()
    {
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(PlayDialogues(helloDialogues));

        StartCoroutine(BackgroundFadeOut());
        glitcher.GetComponent<AudioSource>().Play();
        yield return StartCoroutine(glitcher.Glitching(false));

        GridA.Instance.FillTutorialLayout();
        yield return StartCoroutine(PlayDialogues(blocksDialogues));

        GridA.Instance.currState = GameState.move;
        var movingCrosshair = StartCoroutine(MoveCrosshair());
        yield return new WaitWhile(() => GridA.Instance.allBoxes[2, 7] != null);
        StopCoroutine(movingCrosshair);

        yield return StartCoroutine(PlayDialogues(finalDialogues));
        crosshair.position = new Vector2(-10, 0);
        GridA.Instance.SetDefaultTemplate();
        AudioController.Instance.SetCurrentClip(worldClip, 3.5f);

        tutorialCanvas.SetActive(false);
        //gameCanvas.SetActive(true);
        //UI_System.Instance.m_StartScreen.gameObject.SetActive(true);
        uiSystem.SwitchScreens(gameUI);
        visualizerCanvas.SetActive(true);
        logo.SetActive(true);

        PlayerPrefs.SetInt("TUTORIAL", 1);
        glitcher.SetBombExplodeParameters();

        Destroy(gameObject);
        Destroy(tutorialCanvas);
    }
    IEnumerator PlayDialogues(Dialogue[] dialogues)
    {
        foreach (var dialogue in dialogues)
        {
            text.Clear();
            yield return new WaitForSeconds(0.15f);
            text.PlayMessage(dialogue.Text);
            yield return new WaitWhile(() => text.PlayingMessage);
            yield return new WaitForSeconds(0.25f);
        }
    }

    IEnumerator BackgroundFadeOut()
    {
        yield return new WaitForSeconds(glitcher.glitchTime / 7);
        Image image = background.GetComponent<Image>();

        while (image.color.a > 0)
        {
            Color imageColor = image.color;
            imageColor.a -= 0.05f;
            image.color = imageColor;

            yield return new WaitForSeconds(0.05f);
        }
        background.SetActive(false);
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