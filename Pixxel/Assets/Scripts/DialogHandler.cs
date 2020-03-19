using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogHandler : MonoBehaviour
{
    [SerializeField] string worldToLoad;
    [SerializeField] int worldNumber;
    [SerializeField] Text storyText;
    [SerializeField] Image charakterImage;
    [SerializeField] DialogState startingState;
    [SerializeField] Button redButton, greenButton;
    [SerializeField] SceneLoader sceneLoader;
    [SerializeField] StartDialogs startStates;

    DialogState currState;

    void Start()
    {
        currState = startingState;
        int currLevelTemplate = SaveSystem.LoadLocalLevelData(worldNumber - 1)._currLevelTemplate;
        print("currLevelTemplate is " + currLevelTemplate);
        currState = startStates.startingDialogs[currLevelTemplate];
        storyText.text = currState.StateDialog;
        charakterImage.sprite = currState.CharakterSprite;
        UpdateButtonText();
    }

    public void GreenButtonFunc()
    {
        if (currState.NextStates.Length == 0 || currState.NextStates[0] == null)
        {
            sceneLoader.LoadConcreteWorld(worldToLoad);
        }
        else
        {
            currState = currState.NextStates[0];
            storyText.text = currState.StateDialog;
            UpdateButtonText();
        }
    }

    public void RedButtonFunc()
    {
        if (currState.NextStates[1] != null)
        {
            currState = currState.NextStates[1];
            storyText.text = currState.StateDialog;
            UpdateButtonText();
        }
    }

    void UpdateButtonText()
    {
        redButton.transform.GetComponentInChildren<Text>().text = currState.RedButtonText;
        greenButton.transform.GetComponentInChildren<Text>().text = currState.GreenButtonText;
    }
}
