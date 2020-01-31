using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSlider : MonoBehaviour
{
    [SerializeField] int addDropCoinChance = 3;
    Slider levelSlider;
    private int currentLevel = 1;
    void Start()
    {
        levelSlider = GetComponent<Slider>();
    }

    public void AddXPtoLevel(float amount)
    {
        levelSlider.value += amount;
        if (levelSlider.value >= levelSlider.maxValue)
        {
            currentLevel++;
            levelSlider.GetComponentInChildren<Text>().text = "Level " + currentLevel;
            levelSlider.value = 0;
            levelSlider.maxValue += 100;
            FindObjectOfType<CoinsDisplay>().GetComponent<CoinsDisplay>().IncreaseCoinDropChance(addDropCoinChance);
        }
    }

    public int GetGameLevel()
    {
        return currentLevel;
    }

    public float GetLevelProgress()
    {
        return levelSlider.value;
    }

    public void LoadLevelSlider(LevelData levelData)
    {
        if (levelData != null)
        {
            currentLevel = levelData.currentLevel;
            if (levelSlider != null)
                levelSlider.value = levelData.levelSliderValue;
        }
    }
}
