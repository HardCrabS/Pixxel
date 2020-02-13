using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUp : MonoBehaviour {
    [SerializeField] Text levelText;
    public IConcreteBonus concreteBonus;
    public BonusButton bonusButton;
    CoinsDisplay coinsDisplay;

	void Start () {
        coinsDisplay = FindObjectOfType<CoinsDisplay>();
	}
    public void UpgradeBoost()
    {
        if(coinsDisplay.GetCoins() >= concreteBonus.GetBoostLevelUpCost())
        {
            coinsDisplay.DecreaseCoins(concreteBonus.GetBoostLevelUpCost());
            concreteBonus.LevelUpBoost();
            int level = concreteBonus.GetBoostLevel();
            levelText.text = "lv " + level;

            SerializableBoost newBoost = new SerializableBoost(concreteBonus.GetType().Name, level);
            SaveSystem.SaveBoost(newBoost);
            bonusButton.UpdateBonusLevelInfo();
        }
    }
}
