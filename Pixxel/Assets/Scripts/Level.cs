using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour 
{
    [SerializeField] private int worldNumber = 1;
    LevelSlider levelSlider;
    Score score;
    private int bestScore;

	void Start () {

    }	
    /*
    public void SaveProgress()
    {
        SaveSystem.SaveGlobalData(levelSlider);
        SerializedLevel level = new SerializedLevel(worldNumber-1, bestScore, true, 1);
        SaveSystem.SaveLocalLevelData(level);
    }

    public void LevelTemplateComplete(int trinketIndex)
    {
        SerializedLevel level = new SerializedLevel(worldNumber - 1, bestScore, true, 1);
        SaveSystem.SaveLocalLevelData(level);
    }
    */
    public void LoadLevel()
    {
        levelSlider = FindObjectOfType<LevelSlider>();
        score = FindObjectOfType<Score>();
        LevelData globalLevelData = SaveSystem.LoadGlobalData();
        SerializedLevel localLevelData = SaveSystem.LoadLocalLevelData(worldNumber-1);
        if (levelSlider != null)
        {
            levelSlider.LoadLevelSlider(globalLevelData);
        }
        if(localLevelData != null)
        {
            bestScore = localLevelData._bestScore;
            score.LoadBestScore(this);
        }
    }

    public void SetBestScore(int value)
    {
        bestScore = value;
    }

    public int GetBestScore()
    {
        return bestScore;
    }
}
