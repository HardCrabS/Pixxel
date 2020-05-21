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

    GridA grid;

    [SerializeField] GameObject goalParent;
    LevelSettingsKeeper settingsKeeper;

    private delegate void LevelWon();
    LevelWon onLevelWon;

    // Use this for initialization
    void Start()
    {
        grid = FindObjectOfType<GridA>();
        //settingsKeeper = FindObjectOfType<LevelSettingsKeeper>();
        if (LevelSettingsKeeper.settingsKeeper.levelTemplate.isLeaderboard)
            Destroy(gameObject);
        levelGoals = LevelSettingsKeeper.settingsKeeper.levelTemplate.levelGoals;

        SetupGoals();

        //onLevelWon = FindObjectOfType<Level>().LevelTemplateComplete;
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
            winPanel.SetActive(true);
            if (GameData.gameData != null)
                GameData.gameData.UnlockTrinket(LevelSettingsKeeper.settingsKeeper.worldIndex, LevelSettingsKeeper.settingsKeeper.trinketIndex);
        }
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
    }
}