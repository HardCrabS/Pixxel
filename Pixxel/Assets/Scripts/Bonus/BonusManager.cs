using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IConcreteBonus
{
    void ExecuteBonus();
    Sprite GetSprite();
    Sprite GetSpriteFromImage();
    void SetBoostLevel(int lvl);
    void LevelUpBoost();
    int GetSpiteIndex();
    string GetUniqueAbility(int lvl);
}

public class BonusManager : MonoBehaviour
{
    [SerializeField] bool isBoostScreen;
    [SerializeField] Boost[] allBoostInfos;
    [SerializeField] Sprite[] boostFrames = new Sprite[4];
    [SerializeField] Image[] boostPanels;

    public BonusButton currButtonSelected;

    BonusButton[] children;
    void Awake()
    {
        Dictionary<string, int> boostLevels = GameData.gameData.saveData.boostLevels;
        children = GetComponentsInChildren<BonusButton>();

        if (!isBoostScreen)
        {
            for (int i = 0; i < children.Length; i++)
            {
                string equipedBoostId = GameData.gameData.saveData.equipedBoosts[children[i].buttonIndex];
                if (GameData.gameData.saveData.slotsForBoostsUnlocked[i])
                {
                    children[i].SetButtonForGame(GetBoostWithId(equipedBoostId));

                    int spriteIndex = ChooseBoostSpriteIndex(GameData.gameData.GetBoostLevel(equipedBoostId));
                    boostPanels[i].sprite = boostFrames[spriteIndex];
                }
            }
        }
        else
        {
            for (int i = 0; i < children.Length; i++)
            {
                string boostId = children[i].boostInfo.GetRewardId();
                if (GameData.gameData.saveData.boostIds.Contains(boostId))
                {
                    children[i].transform.GetChild(0).gameObject.SetActive(false);
                    int spriteIndex = ChooseBoostSpriteIndex(GameData.gameData.GetBoostLevel(boostId));
                    boostPanels[i].sprite = boostFrames[spriteIndex];
                    Boost boostInfo = children[i].boostInfo;
                    if (boostInfo != null && spriteIndex < boostInfo.UpgradeSprites.Length)
                    {
                        children[i].GetComponent<Image>().sprite = boostInfo.UpgradeSprites[spriteIndex];
                    }
                }
                else
                {
                    children[i].GetComponent<Button>().interactable = false;
                    boostPanels[i].sprite = boostFrames[0];
                }
            }
        }
    }

    public Sprite[] GetEquipedBoosts()
    {
        Sprite[] boostSprites = new Sprite[3];
        for (int i = 0; i < GameData.gameData.saveData.equipedBoosts.Count; i++)  //check every equiped slot
        {
            string equipedBoostId = GameData.gameData.saveData.equipedBoosts[i];  //get id of equiped boost

            int spriteIndex = ChooseBoostSpriteIndex(GameData.gameData.GetBoostLevel(equipedBoostId));//sprite according to boost level
            Boost boostInfo = GetBoostWithId(equipedBoostId);
            if (boostInfo != null && spriteIndex < boostInfo.UpgradeSprites.Length)
            {
                boostSprites[i] = boostInfo.UpgradeSprites[spriteIndex];
            }

        }
        return boostSprites;
    }
    Boost GetBoostWithId(string id)
    {
        for (int j = 0; j < allBoostInfos.Length; j++)
        {
            if (allBoostInfos[j].GetRewardId() == id)
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

    public void UpdateBoostSprites(string boostId, int level)
    {
        int index = ChooseBoostSpriteIndex(level);
        currButtonSelected.relatedFrame.sprite = boostFrames[index];

        Boost boostInfo = GetBoostWithId(boostId);
        if (index < boostInfo.UpgradeSprites.Length)
        {
            currButtonSelected.GetComponent<Image>().sprite = boostInfo.UpgradeSprites[index];
        }
    }

    public static Sprite GetBoostImage(Boost boostInfo)
    {
        if (boostInfo != null)
        {
            int level = GameData.gameData.GetBoostLevel(boostInfo.GetRewardId());
            int index = ChooseBoostSpriteIndex(level);
            if (index < boostInfo.UpgradeSprites.Length)
            {
                return boostInfo.UpgradeSprites[index];
            }
        }
        return null;
    }
}