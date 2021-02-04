using UnityEngine;

public class LevelUp : MonoBehaviour 
{
    [SerializeField] ClickOnBoost clickOnBoost;
    [SerializeField] BonusManager bonusManager;
    [SerializeField] EquipButton equipButton;
    [SerializeField] AudioClip boostUpgradeSFX;

    public Boost boostInfo;
    public IConcreteBonus bonus;

    public void UpgradeBoost()
    {
        int level = GameData.gameData.GetBoostLevel(boostInfo.id);
        if (level < 10 && CoinsDisplay.Instance.GetCoins() >= boostInfo.GetUpgradeCost(level))
        {
            CoinsDisplay.Instance.DecreaseCoins(boostInfo.GetUpgradeCost(level));
            GameData.gameData.saveData.boostLevels[boostInfo.id]++;
            GameData.Save();
            level++;
            clickOnBoost.ChangeBoostText(boostInfo, bonus.GetUniqueAbility(level));
            bonusManager.UpdateBoostSprites(boostInfo.id, level);
            equipButton.UpdateEquipedBoosts(boostInfo);

            AudioController.Instance.PlayNewClip(boostUpgradeSFX, 0.5f, transform.position);
        }
    }
}
