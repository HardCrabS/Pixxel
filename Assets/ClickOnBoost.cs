using UnityEngine;
using UnityEngine.UI;

public class ClickOnBoost : MonoBehaviour
{
    [SerializeField] Text boostTitleText;
    [SerializeField] Text descriptionText;
    [SerializeField] Text levelText;
    [SerializeField] Text costText;
    [SerializeField] Text currStatsText;
    [SerializeField] Color darkRed;

    public static ClickOnBoost Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void ChangeBoostText(Boost boost, string stats)
    {
        int boostLevel = GameData.gameData.GetBoostLevel(boost.id);
        boostTitleText.text = boost.id.ToUpper() + " <color=black>LV</color><size=300><color=red>" + boostLevel + "</color></size>"; ;

        if (boostLevel < 10)
        {
            int cost = boost.GetUpgradeCost(boostLevel);
            costText.text = "x" + cost;
            levelText.text = "LV<size=350>" + (boostLevel + 1) + "</size>";
        }
        else
        {
            costText.text = "MAX";
            levelText.text = "LV<size=350>" + boostLevel + "</size>";
        }
        if(boostLevel < 4)
        {
            descriptionText.text = boost.descrlevel1;
        }
        else if(boostLevel < 7)
        {
            descriptionText.text = boost.descrlevel4;
        }
        else if (boostLevel < 10)
        {
            descriptionText.text = boost.descrlevel7;
        }
        else
        {
            descriptionText.text = boost.descrlevel10;
        }

        currStatsText.text = "Cooldown: " + SequentialText.ColorString(boost.GetReloadSpeed(boostLevel) + "s", darkRed);
    }
}