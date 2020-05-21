using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSlider : MonoBehaviour
{
    [SerializeField] int addDropCoinChance = 3;
    Slider levelSlider;
    private int currentLevel = 1;
    int currentSaveBorder = 20;
    void Start()
    {
        UpdateLevelText(currentLevel);
    }

    public void AddXPtoLevel(float amount)
    {
        levelSlider.value += amount;
        if (levelSlider.value >= levelSlider.maxValue)
        {
            currentLevel++;
            UpdateLevelText(currentLevel);

            GameData.gameData.saveData.currentLevel = currentLevel;
            GameData.gameData.saveData.levelXP = 0;
            GameData.gameData.Save();

            levelSlider.value = 0;
            levelSlider.maxValue += 100;
            currentSaveBorder = 20;
            FindObjectOfType<CoinsDisplay>().GetComponent<CoinsDisplay>().IncreaseCoinDropChance(addDropCoinChance);
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
        Text levelText = GetComponentInChildren<Text>();
        levelText.text = "Level " + level;
    }

    public int GetGameLevel()
    {
        return currentLevel;
    }

    public float GetLevelProgress()
    {
        return levelSlider.value;
    }

    public void LoadLevelSlider()
    {
        levelSlider = GetComponent<Slider>();
        if (GameData.gameData != null)
        {
            currentLevel = GameData.gameData.saveData.currentLevel;
            if(currentLevel == 0)
            {
                currentLevel = 1;
                GameData.gameData.saveData.currentLevel = 1;
                GameData.gameData.Save();
            }
            if (levelSlider != null)
            {
                levelSlider.value = GameData.gameData.saveData.levelXP;
            }
        }
    }
}
