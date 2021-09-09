using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSlider : MonoBehaviour
{
    [SerializeField] Text rankText;
    [SerializeField] Text nameText;
    [SerializeField] Slider levelSlider;
    [SerializeField] int incrementForLvlUp = 20;//added to max rank slider value

    int currentLevel = 1;
    int currentSaveBorder = 20;
    float xpMultiplier = 1;

    int initLevel;
    float initXPValue;

    public static LevelSlider Instance;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        LoadLevelSlider();
        UpdateLevelText(currentLevel);

        if (GameData.gameData == null) return;
        if (nameText != null)
        {
            string name;
            string savedName = GameData.gameData.saveData.playerInfo.username;
            if (string.IsNullOrEmpty(savedName))
            {
                name = "Warrior";
            }
            else
            {
                name = GameData.gameData.saveData.playerInfo.username;
            }
            nameText.text = "|\t" + name + "\t|";

            string bannerPath = GameData.gameData.saveData.playerInfo.bannerPath;
            var bannerImage = GetComponent<Image>();
            if (bannerImage)
            {
                ProfileHandler.SetBannerFromString(bannerImage, bannerPath);
            }
        }
        initLevel = currentLevel;
        initXPValue = levelSlider.value;
    }

    public void UpdateNameText()
    {
        nameText.text = "|\t" + GameData.gameData.saveData.playerInfo.username + "\t|";
    }

    public void AddXPtoLevel(float amount)
    {
        if (levelSlider == null) return;
        levelSlider.value += amount * xpMultiplier;

        if (levelSlider.value >= levelSlider.maxValue)
        {
            currentLevel++;
            RewardForLevel.Instance.CheckForReward(currentLevel);
            UpdateLevelText(currentLevel);

            GameData.gameData.saveData.currentLevel = currentLevel;
            GameData.gameData.saveData.levelXP = 0;

            levelSlider.value = 0;
            if (currentLevel < 30)
            {
                levelSlider.maxValue += incrementForLvlUp;
            }
            else
            {
                levelSlider.maxValue += 50;
            }
            currentSaveBorder = 20;
            GameData.gameData.saveData.maxXPforLevelUp = levelSlider.maxValue;
            GameData.Save();
            CoinsDisplay.Instance.IncreaseCoinDropChance();
        }
        if (levelSlider.value > currentSaveBorder)
        {
            GameData.gameData.saveData.levelXP = levelSlider.value;
            GameData.Save();
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
    public Tuple<int, float> GetInitLevelInfo()
    {
        return Tuple.Create(initLevel, initXPValue);
    }

    public void LoadLevelSlider()
    {
        if (GameData.gameData != null)
        {
            currentLevel = GameData.gameData.saveData.currentLevel;
            levelSlider.maxValue = GameData.gameData.saveData.maxXPforLevelUp;
            if (currentLevel == 0)
            {
                levelSlider.maxValue = 200;
                GameData.gameData.saveData.maxXPforLevelUp = 200;
                GameData.Save();
            }
            if (levelSlider != null)
            {
                levelSlider.value = GameData.gameData.saveData.levelXP;
            }
        }
    }
}