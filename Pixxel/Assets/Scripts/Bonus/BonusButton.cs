using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BonusButton : MonoBehaviour
{
    [SerializeField] ClickOnBoost clickOnBoost;
    [SerializeField] bool isGameButton;
    public Boost boostInfo;
    public int buttonIndex;
    Animation buttonReloadAnim;

    IConcreteBonus concreteBonus;

    void Start()
    {
        if (isGameButton)
        {
            if (GameData.gameData.saveData.slotsForBoostsUnlocked[buttonIndex])
            {
                GetComponent<Image>().sprite = BonusManager.GetBoostImage(boostInfo);
                buttonReloadAnim = GetComponent<Animation>();

                int boostLevel = GameData.gameData.saveData.boostLevels[boostInfo.Index];
                Type boostType = Type.GetType(boostInfo.BoostTypeString);
                concreteBonus = gameObject.AddComponent(boostType) as IConcreteBonus;
                concreteBonus.SetBoostLevel(boostLevel);

                foreach (AnimationState state in buttonReloadAnim)
                {
                    state.speed = 1 / boostInfo.ReloadSpeed[boostLevel - 1];
                }

            }
            else
            {
                GetComponent<Button>().interactable = false;
            }
        }
    }
    public void SetButtonForGame(Boost boost)
    {
        boostInfo = boost;
    }

    EquipButton equipButton;
    public void GetBoostInfo()
    {
        if(boostInfo == null)
        {
            Debug.LogError("No boostInfo found in button, assign it in the inspector");
            return;
        }
        int level = GameData.gameData.saveData.boostLevels[boostInfo.Index];

        IConcreteBonus myBonus = GetComponent<IConcreteBonus>();

        clickOnBoost.ChangeBoostText(boostInfo, myBonus.GetUniqueAbility(level));
        LevelUp levelUp = FindObjectOfType<LevelUp>();
        levelUp.boostInfo = boostInfo;
        levelUp.bonus = myBonus;

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
        LevelUp levelUp = FindObjectOfType<LevelUp>();
        Boost boostInfo = levelUp.boostInfo;
        IConcreteBonus bonusToEquip = levelUp.bonus;
        if (bonusToEquip != null)
        {
            GetComponent<Image>().sprite = BonusManager.GetBoostImage(boostInfo);

            GameData.gameData.saveData.equipedBoostIndexes[buttonIndex] = boostInfo.Index;
            GameData.gameData.Save();
        }
    }
}