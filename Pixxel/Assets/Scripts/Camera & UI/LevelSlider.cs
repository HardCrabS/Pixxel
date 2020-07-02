using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSlider : MonoBehaviour
{
    [SerializeField] int addDropCoinChance = 3;
    [SerializeField] RewardForLevel levelRewarder;
    [SerializeField] Text rankText;

    Slider levelSlider;
    private int currentLevel = 1;
    int currentSaveBorder = 20;
    CoinsDisplay coinsDisplay;

    void Awake()
    {
        
    }
    void Start()
    {
        LoadLevelSlider();
        UpdateLevelText(currentLevel);
        coinsDisplay = FindObjectOfType<CoinsDisplay>();
    }

    public void AddXPtoLevel(float amount)
    {
        levelSlider.value += amount;
        if (levelSlider.value >= levelSlider.maxValue)
        {
            currentLevel++;
            levelRewarder.CheckForReward(currentLevel);
            UpdateLevelText(currentLevel);

            GameData.gameData.saveData.currentLevel = currentLevel;
            GameData.gameData.saveData.levelXP = 0;

            levelSlider.value = 0;
            levelSlider.maxValue += 100;
            currentSaveBorder = 20;
            GameData.gameData.saveData.maxXPforLevelUp = levelSlider.maxValue;
            GameData.gameData.Save();
            coinsDisplay.IncreaseCoinDropChance(addDropCoinChance);
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

    public void LoadLevelSlider()
    {
        levelSlider = GetComponent<Slider>();
        if (GameData.gameData != null)
        {
            currentLevel = GameData.gameData.saveData.currentLevel;
            levelSlider.maxValue = GameData.gameData.saveData.maxXPforLevelUp;
            if (currentLevel == 0)
            {
                currentLevel = 1;
                levelSlider.maxValue = 200;
                GameData.gameData.saveData.currentLevel = 1;
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
