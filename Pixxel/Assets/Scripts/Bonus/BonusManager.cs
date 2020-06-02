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

    BonusButton[] children;
    void Awake()
    {
        int[] boostLevels = GameData.gameData.saveData.boostLevels;
        children = GetComponentsInChildren<BonusButton>();

        if (!isBoostScreen)
        {
            for (int i = 0; i < children.Length; i++)
            {
                int equipedBoostIndex = GameData.gameData.saveData.equipedBoostIndexes[children[i].buttonIndex];
                if (GameData.gameData.saveData.slotsForBoostsUnlocked[i] && equipedBoostIndex >= 0)
                {
                    if (i < allBoostInfos.Length && allBoostInfos[i].Index < boostLevels.Length)
                        children[i].SetButtonForGame(allBoostInfos[equipedBoostIndex], boostLevels[equipedBoostIndex]);

                    int spriteIndex = ChooseBoostSpriteIndex(boostLevels[equipedBoostIndex]);
                    boostPanels[i].sprite = boostFrames[spriteIndex];
                }
            }
        }
        else
        {
            for (int i = 0; i < children.Length; i++)
            {
                if (GameData.gameData.saveData.boostsUnlocked[i])
                {
                    children[i].transform.GetChild(0).gameObject.SetActive(false);
                    int spriteIndex = ChooseBoostSpriteIndex(boostLevels[i]);
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
        for (int i = 0; i < 3; i++)
        {
            int equipedBoostIndex = GameData.gameData.saveData.equipedBoostIndexes[i];
            if (equipedBoostIndex >= 0)
            {
                int spriteIndex = ChooseBoostSpriteIndex(GameData.gameData.saveData.boostLevels[equipedBoostIndex]);
                Boost boostInfo = allBoostInfos[equipedBoostIndex];
                if (boostInfo != null && spriteIndex < boostInfo.UpgradeSprites.Length)
                {
                    boostSprites[i] = boostInfo.UpgradeSprites[spriteIndex];
                }
            }
        }
        return boostSprites;
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

    public void UpdateBoostSprites(int boostIndex, int level)
    {
        int index = ChooseBoostSpriteIndex(level);
        boostPanels[boostIndex].sprite = boostFrames[index];

        Boost boostInfo = children[boostIndex].boostInfo;
        if (index < boostInfo.UpgradeSprites.Length)
        {
            children[boostIndex].GetComponent<Image>().sprite = boostInfo.UpgradeSprites[index];
        }
    }

    public static Sprite GetBoostImage(Boost boostInfo)
    {
        int level = GameData.gameData.saveData.boostLevels[boostInfo.Index];
        int index = ChooseBoostSpriteIndex(level);

        if (index < boostInfo.UpgradeSprites.Length)
        {
            return boostInfo.UpgradeSprites[index];
        }
        return null;
    }
}