using System.Collections;
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
    [SerializeField] LevelGoal[] levelGoals;
    [SerializeField] List<GoalPanel> currentGoals = new List<GoalPanel>();
    [SerializeField] GameObject goalPrefab;
    [SerializeField] GameObject winPanel;

    [SerializeField] GameObject goalParent;

    private delegate void LevelWon();
    LevelWon onLevelWon;

    List<QuestProgress> dailyQuests;

    public static GoalManager Instance;

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        dailyQuests = new List<QuestProgress>();
        string worldId = LevelSettingsKeeper.settingsKeeper.worldId;
        QuestProgress[] questGoals = GameData.gameData.saveData.dailyQuests;
        for (int i = 0; i < questGoals.Length; i++)
        {
            if(questGoals[i].worldId == worldId)
            {
                dailyQuests.Add(questGoals[i]);
            }
        }
        if (!LevelSettingsKeeper.settingsKeeper.levelTemplate.isLeaderboard)
        {
            levelGoals = LevelSettingsKeeper.settingsKeeper.levelTemplate.levelGoals;
            onLevelWon += DisplayWinPanel;
            SetupGoals();
        }
    }

    void SetupGoals()
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            levelGoals[i].numberCollected = 0;
            GameObject goal = Instantiate(goalPrefab, goalParent.transform.position, Quaternion.identity, goalParent.transform);
            GoalPanel goalPanel = goal.GetComponent<GoalPanel>();
            currentGoals.Add(goalPanel);
            goalPanel.SetPanelSprite(levelGoals[i].goalSprite);
            goalPanel.SetPanelText("0/" + levelGoals[i].numberNeeded);
        }
    }

    public void UpdateGoals()
    {
        int goalsCompleted = 0;

        for (int i = 0; i < levelGoals.Length; i++)
        {
            currentGoals[i].SetPanelText(levelGoals[i].numberCollected + "/" + levelGoals[i].numberNeeded);
            if (levelGoals[i].numberCollected >= levelGoals[i].numberNeeded)
            {
                goalsCompleted++;
                currentGoals[i].SetPanelText(levelGoals[i].numberNeeded + "/" + levelGoals[i].numberNeeded);
            }
        }
        if (goalsCompleted >= levelGoals.Length)
        {
            if (onLevelWon != null)
            {
                onLevelWon();
                onLevelWon -= DisplayWinPanel;
            }
        }
    }

    void DisplayWinPanel()
    {
        RewardForLevel.Instance.CheckForLevelUpReward();
        winPanel.SetActive(true);
        if (GameData.gameData != null)
            GameData.gameData.UnlockTrinket(LevelSettingsKeeper.settingsKeeper.levelTemplate.GetRewardId());
    }

    public void CompareGoal(string goalToCompare, int pointsToAdd = 1)
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            if (levelGoals[i].matchTag == goalToCompare)
            {
                levelGoals[i].numberCollected += pointsToAdd;
            }
        }
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
                break;
            }
        }
    }
}