using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public int currentLevel;
    public float levelSliderValue;
    public SerializedLevel[] levels;

    public LevelData(LevelSlider levelSlider)
    {
        currentLevel = levelSlider.GetGameLevel();
        levelSliderValue = levelSlider.GetLevelProgress();
    }

    public LevelData()
    {
        levels = new SerializedLevel[10];
    }

    public void SaveLevelData(SerializedLevel level)
    {
        levels[level._worldNumber] = level;
    }
}

[System.Serializable]
public class SerializedLevel
{
    public int _worldNumber;
    public int _bestScore;
    public SerializedLevel(int w_number, int bestScore)
    {
        _worldNumber = w_number;
        _bestScore = bestScore;
    }
}