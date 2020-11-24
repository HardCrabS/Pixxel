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
            string trinkName = RewardTemplate.SplitCamelCase(levelTemplate.GetRewardId());
            textChanger.ChangeTrinketTextName(trinkName);
            textChanger.ChangeTrinketTextCondition(levelTemplate.requirementsExplained);
            SetTrinketIndex();
        }
    }

    public void SetSelectionFrame(Transform transform)
    {
        selectionFrame = transform;
    }
    void SetTrinketIndex()
    {
        LevelSettingsKeeper.settingsKeeper.levelTemplate = levelTemplate;
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
        GetComponent<Button>().colors = colors;
    }
}
