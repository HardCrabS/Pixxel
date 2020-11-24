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
        SerializedLevel level1 = new SerializedLevel(0, 0, true);
        SerializedLevel level2 = new SerializedLevel(1, 0, true);
        levels[0] = level1;
        levels[1] = level2;
    }

    public void SaveLevelData(int _worldNumber, int _bestScore, bool _isUnlocked)
    {
        levels[_worldNumber]._bestScore = _bestScore;
        levels[_worldNumber]._isUnlocked = _isUnlocked;
    }
}

[System.Serializable]
public class SerializedLevel
{
    public int _worldNumber;
    public int _bestScore;
    public bool _isUnlocked;
    public SerializedLevel(int w_number, int bestScore, bool isUnlocked)
    {
        _worldNumber = w_number;
        _bestScore = bestScore;
        _isUnlocked = isUnlocked;
    }
}