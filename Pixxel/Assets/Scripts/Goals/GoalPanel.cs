using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalPanel : MonoBehaviour
{
    [SerializeField] Image goalImage;
    [SerializeField] Text goalText;

    public void SetPanelSprite(Sprite goalSprite)
    {
        goalImage.sprite = goalSprite;
    }

    public void SetPanelText(string goalString)
    {
        goalText.text = goalString;
    }
}
