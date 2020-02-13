using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BonusButton : MonoBehaviour
{
    [SerializeField] Sprite[] boostSprites;
    [SerializeField] int buttonIndex;
    [SerializeField] bool isGameButton;
    int level = 1;
    Animation buttonReloadAnim;
    ButtonData bonus;
    IConcreteBonus concreteBonus;

    void Start()
    {
        if (isGameButton)
        {
            GetComponent<Image>().sprite = EquipButton.AssignSpriteToBonus(bonus);
            buttonReloadAnim = GetComponent<Animation>();
            if (bonus != null)
            {
                concreteBonus = gameObject.AddComponent(bonus.Type) as IConcreteBonus;
                concreteBonus.SetBoostLevel(level);

                foreach (AnimationState state in buttonReloadAnim)
                {
                    state.speed = 1 / concreteBonus.TimeToReload();
                }
            }
        }
        else
        {
            if (boostSprites.Length > 0)
            {
                GetComponent<IConcreteBonus>().SetBoostLevel(level);
                GetComponent<Image>().sprite = boostSprites[GetComponent<IConcreteBonus>().GetSpiteIndex()];
            }
        }
    }

    public void UpdateBonusLevelInfo()
    {
        GetComponent<IConcreteBonus>().SetBoostLevel(++level);
        foreach (Button button in equipButton.equipeButtons)
        {
            if (button.GetComponent<Image>().sprite == GetComponent<Image>().sprite)
            {
                equipButton.bonusSprite = boostSprites[GetComponent<IConcreteBonus>().GetSpiteIndex()];
                GetComponent<Image>().sprite = boostSprites[GetComponent<IConcreteBonus>().GetSpiteIndex()];
                GameObject.Find("Boost Image").GetComponent<Image>().sprite = GetComponent<Image>().sprite;
                button.GetComponent<BonusButton>().EquipBonus();
                return;
            }
        }
        GetComponent<Image>().sprite = boostSprites[GetComponent<IConcreteBonus>().GetSpiteIndex()];
        GameObject.Find("Boost Image").GetComponent<Image>().sprite = GetComponent<Image>().sprite;
    }

    public void SetMyBonus(ButtonData buttonData, int lvl)
    {
        bonus = buttonData;
        level = lvl;
    }

    EquipButton equipButton;
    public void GetBoostInfo()
    {
        level = SaveSystem.LoadAllBonuses().GetLevel(gameObject.GetComponent<IConcreteBonus>().GetType());

        IConcreteBonus myBonus = GetComponent<IConcreteBonus>();
        GameObject.Find("Boost Image").GetComponent<Image>().sprite = GetComponent<Image>().sprite;
        GameObject.Find("Boost Title Text").GetComponent<Text>().text = myBonus.GetBoostTitle();
        GameObject.Find("Boost Level Text").GetComponent<Text>().text = "lv " + level;
        GameObject.Find("Boost Description Text").GetComponent<Text>().text = myBonus.GetBoostDescription();
        GameObject.Find("Cost Text").GetComponent<Text>().text = "Cost: " + myBonus.GetBoostLevelUpCost();
        LevelUp levelUp = FindObjectOfType<LevelUp>();
        levelUp.concreteBonus = myBonus;
        levelUp.bonusButton = this;

        if (equipButton == null)
            equipButton = FindObjectOfType<EquipButton>();
        equipButton.currentBonus = myBonus;
        equipButton.bonusSprite = GetComponent<Image>().sprite;
    }

    public void ActivateBonus()
    {
        if (concreteBonus != null)
        {
            concreteBonus.ExecuteBonus();
            buttonReloadAnim.Play();
            GetComponent<Button>().interactable = false;
        }
    }

    public void MakeButtonActive()
    {
        GetComponent<Button>().interactable = true;
    }
    public void EquipBonus()
    {
        EquipButton equipButton = FindObjectOfType<EquipButton>();
        IConcreteBonus bonusToEquip = equipButton.currentBonus;
        if (bonusToEquip != null)
        {
            GetComponent<Image>().sprite = equipButton.bonusSprite;
            equipButton.Equip();

            ButtonData[] allButtons = SaveSystem.LoadEquipedBonuses();
            string name = bonusToEquip.GetSpriteFromImage().name;
            print(bonusToEquip.GetSpriteFromImage().name);
            allButtons[buttonIndex] = new ButtonData(bonusToEquip.GetType(), name);
            SaveSystem.SaveChosenBoosts(allButtons);
        }
    }
}