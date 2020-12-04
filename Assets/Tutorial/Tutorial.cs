using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour 
{
    [SerializeField] GameObject gameCanvas;
    [SerializeField] GameObject visualizerCanvas;
    [SerializeField] GameObject tutorialCanvas;
    [SerializeField] SequentialText text;
    [SerializeField] Dialogue[] helloDialogues;
    [SerializeField] Dialogue[] blocksDialogues;
    [SerializeField] Dialogue[] finalDialogues;

    [SerializeField] Glitcher glitcher;
    [SerializeField] GameObject background;

    void Awake()
    {
        if (PlayerPrefs.GetInt("TUTORIAL", 0) == 0)
        {
            visualizerCanvas.SetActive(false);
            gameCanvas.SetActive(false);
            tutorialCanvas.SetActive(true);
            StartCoroutine(PlayTutorial());
        }
        else
        {
            Destroy(tutorialCanvas);
            Destroy(gameObject);
        }
    }

    public IEnumerator PlayTutorial()
    {
        //AddGlitchComponents();
        yield return StartCoroutine(PlayDialogues(helloDialogues));

        StartCoroutine(BackgroundFadeOut());
        glitcher.GetComponent<AudioSource>().Play();
        yield return StartCoroutine(glitcher.Glitching(false));

        GridA.Instance.FillTutorialLayout();
        yield return StartCoroutine(PlayDialogues(blocksDialogues));
        GridA.Instance.currState = GameState.move;
        yield return new WaitWhile(() => GridA.Instance.allBoxes[2, 7] != null);
        yield return StartCoroutine(PlayDialogues(finalDialogues));

        GridA.Instance.SetDefaultTemplate();
        MusicSing.Instance.PlayClip();

        tutorialCanvas.SetActive(false);
        gameCanvas.SetActive(true);
        visualizerCanvas.SetActive(true);

        PlayerPrefs.SetInt("TUTORIAL", 1);

        Destroy(gameObject);
        Destroy(tutorialCanvas);
        Destroy(glitcher);
    }

    void AddGlitchComponents()
    {
        Camera.main.gameObject.AddComponent<Kino.DigitalGlitch>();
        Camera.main.gameObject.AddComponent<Kino.AnalogGlitch>();
    }

    IEnumerator PlayDialogues(Dialogue[] dialogues)
    {
        foreach(var dialogue in dialogues)
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
}