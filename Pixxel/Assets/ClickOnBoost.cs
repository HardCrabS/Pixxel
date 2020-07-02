using System.Collections;
using System.Collections.Generic;
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
        boostTitleText.text = boost.Title;
        descriptionText.text = boost.Description;
        int boostLevel = GameData.gameData.saveData.boostLevels[boost.Index];
        levelText.text = "lv " + boostLevel;
        if (boostLevel < boost.UpgradeCosts.Length - 1)
        {
            int cost = boost.UpgradeCosts[boostLevel - 1];
            costText.text = "" + cost;
        }
        else
        {
            costText.text = "max";
        }
        string[] allStats = stats.Split('|');
        currStatsText.text = "Cooldown: " + "<color=red>" + boost.ReloadSpeed[boostLevel - 1] + "s</color>\n" + allStats[0];
        if (boostLevel < boost.ReloadSpeed.Length)
        {
            if (allStats.Length > 1)
            {
                nextStatsText.text = "Cooldown: " + "<color=red>" + boost.ReloadSpeed[boostLevel] + "s</color>\n" + allStats[1];
            }
            else
            {
                nextStatsText.text = "Cooldown: " + "<color=red>" + boost.ReloadSpeed[boostLevel] + "s</color>\n";
            }
        }
        else
            nextStatsText.text = "Maximum level reached!";
    }

    public void UpdateText(Boost boost, string stats)
    {
        int boostLevel = GameData.gameData.saveData.boostLevels[boost.Index];
        levelText.text = "lv " + boostLevel;
        if (boostLevel < boost.UpgradeCosts.Length + 1)
        {
            int cost = boost.UpgradeCosts[boostLevel - 1];
            costText.text = "Cost: " + cost;
        }
        else
        {
            costText.text = "max";
        }

        string[] allStats = stats.Split('|');
        currStatsText.text = "Cooldown: " + "<color=red>" + boost.ReloadSpeed[boostLevel - 1] + "s</color>\n" + allStats[0];
        if (boostLevel < boost.ReloadSpeed.Length)
        {
            if (allStats.Length > 1)
            {
                nextStatsText.text = "Cooldown: " + "<color=red>" + boost.ReloadSpeed[boostLevel] + "s</color>\n" + allStats[1];
            }
            else
            {
                nextStatsText.text = "Cooldown: " + "<color=red>" + boost.ReloadSpeed[boostLevel] + "s</color>\n";
            }
        }
        else
            nextStatsText.text = "Maximum level reached!";
    }
}
