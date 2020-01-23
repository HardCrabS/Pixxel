using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUp : MonoBehaviour {
    [SerializeField] Text levelText;
    public IConcreteBonus concreteBonus;
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
            print("I saved " + level);
            SerializableBoost newBoost = new SerializableBoost(concreteBonus.GetType().Name, level);
            Bonus b = SaveSystem.LoadAllBonuses();
            for (int i = 0; i < 2; i++)
            {
                print(b.boosts[i].stringType);
            }
            print("type to save is " + concreteBonus.GetType());
            SaveSystem.SaveBoost(newBoost);
        }
    }
}
