using UnityEngine;

public class LevelUp : MonoBehaviour 
{
    [SerializeField] ClickOnBoost clickOnBoost;
    [SerializeField] BonusManager bonusManager;
    [SerializeField] EquipButton equipButton;

    public Boost boostInfo;
    public IConcreteBonus bonus;

    public void UpgradeBoost()
    {
        int level = GameData.gameData.GetBoostLevel(boostInfo.GetRewardId());
        if (level < 10 && CoinsDisplay.Instance.GetCoins() >= boostInfo.UpgradeCosts[level - 1])
        {
            CoinsDisplay.Instance.DecreaseCoins(boostInfo.UpgradeCosts[level - 1]);
            GameData.gameData.saveData.boostLevels[boostInfo.GetRewardId()]++;
            GameData.gameData.Save();
            level++;
            clickOnBoost.UpdateText(boostInfo, bonus.GetUniqueAbility(level));
            bonusManager.UpdateBoostSprites(boostInfo.GetRewardId(), level);
            equipButton.UpdateEquipedBoosts(boostInfo);
        }
    }
}
