using UnityEngine;
using UnityEngine.UI;

public class ClickOnBoost : MonoBehaviour
{
    [SerializeField] Text boostTitleText;
    [SerializeField] Text descriptionText;
    [SerializeField] Text levelText;
    [SerializeField] Text costText;
    [SerializeField] Text currStatsText;
    [SerializeField] Text nextStatsText;

    public void ChangeBoostText(Boost boost, string stats)
    {
        boostTitleText.text = boost.id;
        descriptionText.text = boost.description;
        int boostLevel = GameData.gameData.GetBoostLevel(boost.id);
        levelText.text = "lv " + boostLevel;
        if (boostLevel < 10)
        {
            int cost = boost.GetUpgradeCost(boostLevel);
            costText.text = "" + cost;
        }
        else
        {
            costText.text = "MAX";
        }
        string[] allStats = stats.Split('|');
        currStatsText.text = "Cooldown: " + "<color=red>" + boost.GetReloadSpeed(boostLevel) + "s</color>\n" + allStats[0];
        if (boostLevel < 10)
        {
            if (allStats.Length > 1)
            {
                nextStatsText.text = "Cooldown: " + "<color=red>" + boost.GetReloadSpeed(boostLevel + 1) + "s</color>\n" + allStats[1];
            }
            else
            {
                nextStatsText.text = "Cooldown: " + "<color=red>" + boost.GetReloadSpeed(boostLevel + 1) + "s</color>\n";
            }
        }
        else
            nextStatsText.text = "Maximum level reached!";
    }

    public void UpdateText(Boost boost, string stats)
    {
        int boostLevel = GameData.gameData.GetBoostLevel(boost.id);
        levelText.text = "lv " + boostLevel;
        if (boostLevel < 10)
        {
            int cost = boost.GetUpgradeCost(boostLevel);
            costText.text = "" + cost;
        }
        else
        {
            costText.text = "MAX";
        }

        string[] allStats = stats.Split('|');
        currStatsText.text = "Cooldown: " + "<color=red>" + boost.GetReloadSpeed(boostLevel) + "s</color>\n" + allStats[0];
        if (boostLevel < 10)
        {
            if (allStats.Length > 1)
            {
                nextStatsText.text = "Cooldown: " + "<color=red>" + boost.GetReloadSpeed(boostLevel + 1) + "s</color>\n" + allStats[1];
            }
            else
            {
                nextStatsText.text = "Cooldown: " + "<color=red>" + boost.GetReloadSpeed(boostLevel + 1) + "s</color>\n";
            }
        }
        else
            nextStatsText.text = "Maximum level reached!";
    }
}