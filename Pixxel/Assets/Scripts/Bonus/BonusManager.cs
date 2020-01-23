using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IConcreteBonus
{
    void ExecuteBonus();
    Sprite GetSprite();
    Sprite GetSpriteFromImage();
    float TimeToReload();
    string GetBoostTitle();
    string GetBoostDescription();
    int GetBoostLevel();
    void SetBoostLevel(int lvl);
    void LevelUpBoost();
    int GetBoostLevelUpCost();
}

public class BonusManager : MonoBehaviour
{
    [SerializeField] bool isBoostScreen;
    public ButtonData[] bonusArray = new ButtonData[3];
    void Awake()
    {
        bonusArray = SaveSystem.LoadConcreteBonuses();
        Bonus bonusesInfo = SaveSystem.LoadAllBonuses();
        BonusButton[] children = GetComponentsInChildren<BonusButton>();

        if (bonusesInfo != null && bonusArray != null)
        {
            if (!isBoostScreen)
            {
                for (int i = 0; i < bonusArray.Length && i < children.Length; i++)
                {
                    if (bonusArray[i] != null)
                    {
                        int level = bonusesInfo.GetLevel(bonusArray[i].Type);
                        children[i].SetMyBonus(bonusArray[i], level);
                    }
                }
            }
            else
            {
                for (int i = 0; i < children.Length; i++)
                {
                    int level = bonusesInfo.GetLevel(children[i].gameObject.GetComponent<IConcreteBonus>().GetType());
                    print("type is " + children[i].GetType() + "; level is " + level);
                    children[i].SetMyBonus(bonusArray[i], level);
                }
            }
        }
    }
}
