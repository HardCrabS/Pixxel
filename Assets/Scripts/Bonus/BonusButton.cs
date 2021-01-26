using System;
using UnityEngine;
using UnityEngine.UI;

public class BonusButton : MonoBehaviour
{
    [SerializeField] ClickOnBoost clickOnBoost;
    [SerializeField] bool isGameButton;
    public Image relatedFrame;
    public Boost boostInfo;
    public int buttonIndex; // for equip slots
    Animation buttonReloadAnim;

    IConcreteBonus concreteBonus;

    void Start()
    {
        if (isGameButton)
        {
            bool isUnlocked = GameData.gameData == null ? false : GameData.gameData.saveData.slotsForBoostsUnlocked[buttonIndex];
            if (isUnlocked)
            {
                if (boostInfo != null)
                {
                    GetComponent<Image>().color = Color.white;
                    GetComponent<Image>().sprite = BonusManager.GetBoostImage(boostInfo);
                    buttonReloadAnim = GetComponent<Animation>();

                    int boostLevel = GameData.gameData.GetBoostLevel(boostInfo.id);
                    Type boostType = Type.GetType(boostInfo.BoostTypeString);
                    concreteBonus = gameObject.AddComponent(boostType) as IConcreteBonus;
                    concreteBonus.SetBoostLevel(boostLevel);

                    foreach (AnimationState state in buttonReloadAnim)
                    {
                        state.speed = 1 / boostInfo.GetReloadSpeed(boostLevel);
                    }
                }
                else
                {
                    gameObject.SetActive(false); //hide bonus button if wasnt picked
                }
            }
            else
            {
                Button button = GetComponent<Button>(); //set lock image if panel is locked
                button.interactable = false;
                var colors = button.colors;
                colors.disabledColor = Color.white;
                button.colors = colors;
                gameObject.AddComponent<Shadow>().effectDistance = new Vector2(10, -10);
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
        int level = GameData.gameData.GetBoostLevel(boostInfo.id);

        IConcreteBonus myBonus = GetComponent<IConcreteBonus>();

        clickOnBoost.ChangeBoostText(boostInfo, myBonus.GetUniqueAbility(level));
        LevelUp levelUp = FindObjectOfType<LevelUp>();
        levelUp.boostInfo = boostInfo;
        levelUp.bonus = myBonus;

        if (equipButton == null)
            equipButton = FindObjectOfType<EquipButton>();
        equipButton.currentBonus = myBonus;
        equipButton.bonusSprite = GetComponent<Image>().sprite;
        GetComponentInParent<BonusManager>().currButtonSelected = this;
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

            GameData.gameData.saveData.equipedBoosts[buttonIndex] = boostInfo.id;
            GameData.Save();
        }
    }
}