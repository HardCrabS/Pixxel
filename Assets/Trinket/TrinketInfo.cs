using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrinketInfo : MonoBehaviour 
{
    public LevelTemplate levelTemplate;
    [SerializeField] TextChanger textChanger;

    [SerializeField] ColorBlock inactiveTrinketColors;

    Transform selectionFrame;

    public void SetTrinketButton()
    {
        selectionFrame.position = transform.position;
        selectionFrame.SetParent(transform);
        if (levelTemplate != null)
        {
            string trinkName = levelTemplate.id;
            textChanger.ChangeTrinketTextName(trinkName);

            string trinketId = levelTemplate.id;
            string trinketProgress;

            if(GameData.gameData.saveData.trinketsProgress.ContainsKey(trinketId))
            {
                int numCollected = GameData.gameData.saveData.trinketsProgress[trinketId];
                if (numCollected >= levelTemplate.levelGoal.numberNeeded)
                {
                    numCollected = levelTemplate.levelGoal.numberNeeded;
                    trinketProgress = numCollected + "/" + levelTemplate.levelGoal.numberNeeded;
                    trinketProgress = SequentialText.ColorString(trinketProgress, Color.green);
                }
                else
                {
                    trinketProgress = numCollected + "/" + levelTemplate.levelGoal.numberNeeded;
                    trinketProgress = SequentialText.ColorString(trinketProgress, Color.red);
                }
            }
            else
            {
                trinketProgress = 0 + "/" + levelTemplate.levelGoal.numberNeeded;
                trinketProgress = SequentialText.ColorString(trinketProgress, Color.red);
            }
            textChanger.ChangeTrinketTextCondition(levelTemplate.requirementsExplained 
                + "\n" + trinketProgress);
        }
    }

    public void SetSelectionFrame(Transform transform)
    {
        selectionFrame = transform;
    }

    public void LockTrinket()
    {
        GetComponent<Button>().colors = inactiveTrinketColors;
    }
    public void MakeUnlocked()
    {
        var colors = GetComponent<Button>().colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.95f, 0.95f, 0.95f);
        colors.pressedColor = new Color(0.9f, 0.9f, 0.9f);
        colors.selectedColor = new Color(1, 1, 1, 1);
        GetComponent<Button>().colors = colors;
    }
}
