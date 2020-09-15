using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSlider : MonoBehaviour
{
    [SerializeField] int addDropCoinChance = 3;
    [SerializeField] RewardForLevel levelRewarder;
    [SerializeField] Text rankText;
    [SerializeField] Text nameText;

    Slider levelSlider;
    int currentLevel = 1;
    int currentSaveBorder = 20;
    float xpMultiplier = 1;

    public static LevelSlider Instance;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        LoadLevelSlider();
        UpdateLevelText(currentLevel);

        if (nameText != null)
        {
            string name;
            if (string.IsNullOrEmpty(GameData.gameData.saveData.playerInfo.username))
            {
                name = "Warrior";
            }
            else
            {
                name = GameData.gameData.saveData.playerInfo.username;
            }
            nameText.text = "|\t" + name + "\t|";
        }
    }

    public void UpdateNameText()
    {
        nameText.text = GameData.gameData.saveData.playerInfo.username;
    }

    public void AddXPtoLevel(float amount)
    {
        levelSlider.value += amount * xpMultiplier;
        if (levelSlider.value >= levelSlider.maxValue)
        {
            currentLevel++;
            levelRewarder.CheckForReward(currentLevel);
            UpdateLevelText(currentLevel);

            GameData.gameData.saveData.currentLevel = currentLevel;
            GameData.gameData.saveData.levelXP = 0;

            levelSlider.value = 0;
            levelSlider.maxValue += 10;
            currentSaveBorder = 20;
            GameData.gameData.saveData.maxXPforLevelUp = levelSlider.maxValue;
            GameData.gameData.Save();
            CoinsDisplay.Instance.IncreaseCoinDropChance(addDropCoinChance);
        }
        if (levelSlider.value > currentSaveBorder)
        {
            GameData.gameData.saveData.levelXP = levelSlider.value;
            GameData.gameData.Save();
            currentSaveBorder += 20;
        }
    }

    void UpdateLevelText(int level)
    {
        rankText.text = "Rank " + level;
    }

    public int GetGameLevel()
    {
        return currentLevel;
    }

    public float GetLevelProgress()
    {
        return levelSlider.value;
    }

    public int GetXPforLevelUp()
    {
        return (int)(levelSlider.maxValue - levelSlider.value);
    }

    public void SetXPMultiplier(float multiplier)
    {
        xpMultiplier = multiplier;
    }

    public void LoadLevelSlider()
    {
        levelSlider = GetComponent<Slider>();
        if (GameData.gameData != null)
        {
            currentLevel = GameData.gameData.saveData.currentLevel;
            levelSlider.maxValue = GameData.gameData.saveData.maxXPforLevelUp;
            if (currentLevel == 0)
            {
                levelSlider.maxValue = 200;
                GameData.gameData.saveData.maxXPforLevelUp = 200;
                GameData.gameData.Save();
            }
            if (levelSlider != null)
            {
                levelSlider.value = GameData.gameData.saveData.levelXP;
            }
        }
    }
}
