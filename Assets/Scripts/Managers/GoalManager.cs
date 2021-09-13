using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelGoal
{
    public int numberNeeded;
    [HideInInspector] public int numberCollected = 0;
    public string matchTag;
}

public class GoalManager : MonoBehaviour
{
    [SerializeField] ShareButton shareButton;

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
        string worldId = LevelSettingsKeeper.settingsKeeper.worldLoadInfo.id;
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
        LevelTemplate trinket;
        string trinketId;

        WorldLoadInfo worldLoadInformation = LevelSettingsKeeper.settingsKeeper.worldLoadInfo;
        GameData gamedata = GameData.gameData;
        for (int i = 0; i < worldLoadInformation.trinketTemplates.Length - 1; i++)//add only 9 trinkets, 10th is for sharing
        {
            trinket = worldLoadInformation.trinketTemplates[i];
            trinketId = trinket.id; //get trinket id
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
                levelTemplates.Add(trinket); //getting if not completed 
            }
        }

        //set 10th trinket for sharing
        trinket = worldLoadInformation.trinketTemplates[9];
        trinketId = trinket.id; //get trinket id

        if (gamedata.saveData.trinketsProgress.ContainsKey(trinketId)) //if id already saved
        {
            if (gamedata.saveData.trinketsProgress[trinketId] < trinket.levelGoal.numberNeeded)
            {
                shareButton.OnShareSuccess.AddListener(() => {
                    gamedata.saveData.trinketsProgress[trinketId] = trinket.levelGoal.numberNeeded;
                    gamedata.saveData.trinketIds.Add(trinketId);
                    GameData.Save();
                });
            }
        }
        else
        {
            gamedata.saveData.trinketsProgress.Add(trinketId, 0); //create new element
            shareButton.OnShareSuccess.AddListener(() => {
                gamedata.saveData.trinketsProgress[trinketId] = trinket.levelGoal.numberNeeded;
                gamedata.saveData.trinketIds.Add(trinketId);
                GameData.Save();
            });
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
                string trinketId = levelTemplates[i].id; //get trinket id
                gamedata.saveData.trinketsProgress[trinketId] += pointsToAdd; //update

                if(gamedata.saveData.trinketsProgress[trinketId] >= levelTemplates[i].levelGoal.numberNeeded)
                {
                    gamedata.saveData.trinketsProgress[trinketId] = levelTemplates[i].levelGoal.numberNeeded;
                    gamedata.saveData.trinketIds.Add(trinketId);
                    RewardForLevel.Instance.SpawnReward(levelTemplates[i]);
                    levelTemplates.RemoveAt(i);
                }
                //only score goal in trinkets, so multiple at a time
                //break; //only 1 task with same goal at a time
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