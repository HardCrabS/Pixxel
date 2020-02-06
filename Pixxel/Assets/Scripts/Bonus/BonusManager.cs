using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    int GetSpiteIndex();
}

public class BonusManager : MonoBehaviour
{
    [SerializeField] bool isBoostScreen;
    [SerializeField] GameObject[] panels = new GameObject[3];
    [SerializeField] Sprite[] boostFrames = new Sprite[4];
    public ButtonData[] equipedBonuses = new ButtonData[3];

    void Awake()
    {
        equipedBonuses = SaveSystem.LoadEquipedBonuses();
        Bonus bonusesInfo = SaveSystem.LoadAllBonuses();
        BonusButton[] children = GetComponentsInChildren<BonusButton>();

        if (bonusesInfo != null && equipedBonuses != null)
        {
            if (!isBoostScreen)
            {
                for (int i = 0; i < equipedBonuses.Length && i < children.Length; i++)
                {
                    if (equipedBonuses[i] != null)
                    {
                        int level = bonusesInfo.GetLevel(equipedBonuses[i].Type);
                        children[i].SetMyBonus(equipedBonuses[i], level);

                        if (level < 4)
                        {
                            panels[i].GetComponent<Image>().sprite = boostFrames[0];
                        }
                        else if(level < 7)
                        {
                            panels[i].GetComponent<Image>().sprite = boostFrames[1];
                        }
                        else if(level < 10)
                        {
                            panels[i].GetComponent<Image>().sprite = boostFrames[2];
                        }
                        else
                        {
                            panels[i].GetComponent<Image>().sprite = boostFrames[3];
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < children.Length; i++)
                {
                    int level = bonusesInfo.GetLevel(children[i].gameObject.GetComponent<IConcreteBonus>().GetType());
                    print("type is " + children[i].GetType() + "; level is " + level);
                    children[i].SetMyBonus(null, level);
                }
            }
        }
    }
}
