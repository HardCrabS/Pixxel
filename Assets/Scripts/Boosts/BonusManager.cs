using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusManager : MonoBehaviour
{
    [SerializeField] bool isBoostScreen;
    [SerializeField] Transform selectionGlow;
    [SerializeField] Boost[] allBoostInfos;
    [SerializeField] Sprite[] boostFrames = new Sprite[4];
    [SerializeField] EquipButton equipButton; //main equip buttons manager
    public Sprite lockedFrame;
    public Image[] boostPanels;

    public BonusButton currButtonSelected;

    BonusButton[] bonusButtons;

    public static BonusManager Instance;
    void Awake()
    {
        if (GameData.gameData == null) return;
        if (Instance == null) Instance = this;

        bonusButtons = GetComponentsInChildren<BonusButton>();
        if (isBoostScreen)
        {
            for (int i = 0; i < bonusButtons.Length; i++)   //loop through all boosts in boost screen
            {
                string boostId = bonusButtons[i].boostInfo.id;
                bool isUnlocked = GameData.gameData.saveData.boostIds.Contains(boostId);
                int index = i;
                bonusButtons[i].GetComponent<Button>().onClick.AddListener(
                    () => SetSelectionGlowPos(bonusButtons[index].transform));
                if (isUnlocked)
                {
                    bonusButtons[i].IsUnlocked = isUnlocked;
                    bonusButtons[i].transform.GetChild(0).gameObject.SetActive(false);  //lock image
                    int spriteIndex = ChooseBoostSpriteIndex(GameData.gameData.GetBoostLevel(boostId));
                    //boostPanels[i].sprite = boostFrames[spriteIndex];
                    Boost boostInfo = bonusButtons[i].boostInfo;
                    if (boostInfo != null && spriteIndex < boostInfo.UpgradeSprites.Length)
                    {
                        bonusButtons[i].GetComponent<Image>().sprite = boostInfo.UpgradeSprites[spriteIndex];
                    }
                    bonusButtons[i].gameObject.AddComponent<DragBoost>();                    
                }
                else
                {
                    Color disabledColor = bonusButtons[i].GetComponent<Button>().colors.disabledColor;
                    bonusButtons[i].GetComponent<Image>().color = disabledColor;
                    //boostPanels[i].sprite = lockedFrame;
                }
            }
        }
        else
        {
            for (int i = 0; i < bonusButtons.Length; i++)   //loop through selected boosts in game
            {
                string equipedBoostId = GameData.gameData.saveData.equipedBoosts[bonusButtons[i].buttonIndex];
                if (GameData.gameData.saveData.slotsForBoostsUnlocked[i])
                {
                    bonusButtons[i].SetButtonForGame(GetBoostWithId(equipedBoostId));
                    SetBoostFrame(equipedBoostId, i);
                }
                else
                {
                    boostPanels[i].sprite = lockedFrame;
                }
            }
        }
    }
    void SetSelectionGlowPos(Transform boostTransform)
    {
        selectionGlow.position = boostTransform.position;
    }
    public void SetAllButtonsInterraction(bool state)
    {
        foreach (BonusButton button in bonusButtons)
        {
            if (button != null)
                button.SetInteractable(state);
        }
    }
    public bool BoostIsActivated()//returns true if any boost is activated
    {
        foreach (BonusButton button in bonusButtons)
        {
            if (button.gameObject.activeInHierarchy && !button.BoostIsFinished())
                return true;
        }
        return false;
    }
    public void StopAllBoosts()
    {
        foreach (BonusButton button in bonusButtons)
        {
            if (button.gameObject.activeInHierarchy && !button.BoostIsFinished())
            {
                button.GetComponent<BoostBase>().StopBonus();
            }
        }
    }
    public void CheckIfEquipedInOtherSlot(int buttonIndex, string boostId)
    {
        for (int i = 0; i < GameData.gameData.saveData.equipedBoosts.Count; i++)  //check every equiped slot
        {
            if (i == buttonIndex) continue;
            string equipedBoostId = GameData.gameData.saveData.equipedBoosts[i];  //get id of equiped boost

            if (string.Compare(equipedBoostId, boostId) == 0)    //found equipped button with same boost id
            {
                GameData.gameData.saveData.equipedBoosts[i] = null;
                equipButton.equipeButtons[i].GetComponent<Image>().color = new Color(1, 1, 1, 0);

                boostPanels[i].sprite = boostFrames[0];
            }
        }

        SetBoostFrame(boostId, buttonIndex); //set frame image
    }
    public Tuple<Sprite, Sprite>[] GetEquipedBoostSprites()
    {
        Tuple<Sprite, Sprite>[] boostSpritesAndFrames = new Tuple<Sprite, Sprite>[3];
        //Sprite[] boostSprites = new Sprite[3];
        for (int i = 0; i < GameData.gameData.saveData.equipedBoosts.Count; i++)  //check every equiped slot
        {
            string equipedBoostId = GameData.gameData.saveData.equipedBoosts[i];  //get id of equiped boost

            int spriteIndex = ChooseBoostSpriteIndex(GameData.gameData.GetBoostLevel(equipedBoostId));//sprite according to boost level
            Boost boostInfo = GetBoostWithId(equipedBoostId);
            Sprite boostFrame = GetBoostFrame(equipedBoostId);
            if (boostInfo != null && spriteIndex < boostInfo.UpgradeSprites.Length)
            {
                Tuple<Sprite, Sprite> sprites = new Tuple<Sprite, Sprite>(boostInfo.UpgradeSprites[spriteIndex], boostFrame);
                boostSpritesAndFrames[i] = sprites;
            }
        }
        return boostSpritesAndFrames;
    }
    public Boost[] GetEquipedBoostInfos()
    {
        int boostCount = GameData.gameData.saveData.equipedBoosts.Count;
        Boost[] boostInfos = new Boost[boostCount];
        for (int i = 0; i < boostCount; i++)  //check every equiped slot
        {
            string equipedBoostId = GameData.gameData.saveData.equipedBoosts[i];  //get id of equiped boost
            boostInfos[i] = GetBoostWithId(equipedBoostId);
        }
        return boostInfos;
    }
    public BonusButton GetBonusButton(string boostId)
    {
        foreach (var boostButton in bonusButtons)
        {
            if (boostButton.boostInfo.id == boostId)
                return boostButton;
        }
        return null;
    }
    Boost GetBoostWithId(string id)
    {
        for (int j = 0; j < allBoostInfos.Length; j++)
        {
            if (allBoostInfos[j].id == id)
            {
                return allBoostInfos[j];
            }
        }
        return null;
    }
    public static int ChooseBoostSpriteIndex(int level)
    {
        if (level < 4)
            return 0;
        else if (level < 7)
            return 1;
        else if (level < 10)
            return 2;
        else
            return 3;
    }

    public void UpdateBoostSprites(Boost boostInfo, int level)
    {
        int index = ChooseBoostSpriteIndex(level);

        if (index < boostInfo.UpgradeSprites.Length)
        {
            currButtonSelected.GetComponent<Image>().sprite = boostInfo.UpgradeSprites[index];
        }
    }
    public void SetBoostFrame(string boostId, int buttonIndex)
    {
        int spriteIndex = ChooseBoostSpriteIndex(GameData.gameData.GetBoostLevel(boostId));
        boostPanels[buttonIndex].sprite = boostFrames[spriteIndex];
    }
    public Sprite GetBoostFrame(string boostId)
    {
        int spriteIndex = ChooseBoostSpriteIndex(GameData.gameData.GetBoostLevel(boostId));
        return boostFrames[spriteIndex];
    }
    public static Sprite GetBoostImage(Boost boostInfo)
    {
        if (boostInfo != null)
        {
            int level = GameData.gameData.GetBoostLevel(boostInfo.id);
            int index = ChooseBoostSpriteIndex(level);
            if (index < boostInfo.UpgradeSprites.Length)
            {
                return boostInfo.UpgradeSprites[index];
            }
        }
        return null;
    }
}