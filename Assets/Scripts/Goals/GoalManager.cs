using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelGoal
{
    public int numberNeeded;
    public int numberCollected = 0;
    public Sprite goalSprite;
    public string matchTag;
}

public class GoalManager : MonoBehaviour
{
    List<LevelTemplate> levelTemplates;
    List<QuestProgress> dailyQuests;

    public static GoalManager Instance;

    void Awake()
    {
        if (LevelSettingsKeeper.settingsKeeper == null)
        {
            Destroy(this);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        SetDailyQuests();
        SetupGoals();
    }

    private void SetDailyQuests()
    {
        dailyQuests = new List<QuestProgress>();
        string worldId = LevelSettingsKeeper.settingsKeeper.worldInfo.GetRewardId();
        QuestProgress[] questGoals = GameData.gameData.saveData.dailyQuests;
        for (int i = 0; i < questGoals.Length; i++)
        {
            if (questGoals[i].worldId == worldId)
            {
                dailyQuests.Add(questGoals[i]);
            }
        }
    }

    void SetupGoals()
    {
        levelTemplates = new List<LevelTemplate>();

        WorldInformation worldInformation = LevelSettingsKeeper.settingsKeeper.worldInfo;
        GameData gamedata = GameData.gameData;
        for (int i = 0; i < worldInformation.TrinketLevelTemplates.Length; i++)
        {
            LevelTemplate trinket = worldInformation.TrinketLevelTemplates[i];
            string trinketId = trinket.GetRewardId(); //get trinket id
            if (gamedata.saveData.trinketsProgress.ContainsKey(trinketId)) //if id already saved
            {
                if(gamedata.saveData.trinketsProgress[trinketId] < trinket.levelGoal.numberNeeded)
                {
                    levelTemplates.Add(trinket); //getting if not completed 
                }
            }
            else
            {
                gamedata.saveData.trinketsProgress.Add(trinketId, 0); //create new element
            }
        }
    }

    public void CompareGoal(string goalToCompare, int pointsToAdd = 1)
    {
        GameData gamedata = GameData.gameData;
        //trinkets
        for (int i = 0; i < levelTemplates.Count; i++)
        {
            if (levelTemplates[i].levelGoal.matchTag == goalToCompare) //if goal compares
            {
                string trinketId = levelTemplates[i].GetRewardId(); //get trinket id
                gamedata.saveData.trinketsProgress[trinketId] += pointsToAdd; //update

                if(gamedata.saveData.trinketsProgress[trinketId] >= levelTemplates[i].levelGoal.numberNeeded)
                {
                    gamedata.saveData.trinketsProgress[trinketId] = levelTemplates[i].levelGoal.numberNeeded;
                    gamedata.saveData.trinketIds.Add(trinketId);
                    levelTemplates.RemoveAt(i);
                }
            }
        }

        //daily quests
        for (int i = 0; i < dailyQuests.Count; i++)
        {
            if (dailyQuests[i].tag == goalToCompare)
            {
                var quest = dailyQuests[i];
                quest.numberCollected += pointsToAdd;
                dailyQuests[i] = quest;
                GameData.gameData.saveData.dailyQuests[quest.savedArrayIndex] = quest;

                if (quest.numberCollected >= quest.numberNeeded)
                {
                    dailyQuests.RemoveAt(i);
                }
                break; //only 1 task with same goal at a time
            }
        }
    }
}