using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUp : MonoBehaviour 
{
    [SerializeField] ClickOnBoost clickOnBoost;
    [SerializeField] BonusManager bonusManager;
    [SerializeField] EquipButton equipButton;

    public Boost boostInfo;
    public IConcreteBonus bonus;

    CoinsDisplay coinsDisplay;

	void Start () 
    {
        coinsDisplay = FindObjectOfType<CoinsDisplay>();
	}
    public void UpgradeBoost()
    {
        int level = GameData.gameData.saveData.boostLevels[boostInfo.Index];
        if (level < 10 && coinsDisplay.GetCoins() >= boostInfo.UpgradeCosts[level - 1])
        {
            coinsDisplay.DecreaseCoins(boostInfo.UpgradeCosts[level - 1]);
            GameData.gameData.saveData.boostLevels[boostInfo.Index]++;
            GameData.gameData.Save();
            //bonusButton.UpdateBonusLevelInfo();
            level++;
            clickOnBoost.UpdateText(boostInfo, bonus.GetUniqueAbility(level));
            bonusManager.UpdateBoostSprites(boostInfo.Index, level);
            equipButton.UpdateEquipedBoosts(boostInfo);
        }
    }
}
