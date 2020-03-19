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
    [SerializeField] GameObject goalParent;
    [SerializeField] GameObject okButton;
    GridA grid;

    private delegate void LevelWon();
    LevelWon onLevelWon;


    // Use this for initialization
    void Start()
    {
        grid = FindObjectOfType<GridA>();
        GetGoals();
        SetupGoals();
        onLevelWon = FindObjectOfType<Level>().LevelTemplateComplete;
    }

    public void OKButton(string worldToLoad)
    {
        if (grid.world != null)
        {
            if (grid.world.levels[grid.level] != null)
            {
                if(grid.world.levels[grid.level].loadStoryScene)
                {
                    FindObjectOfType<SceneLoader>().LoadConcreteWorld(worldToLoad);
                }
                else
                {
                    FindObjectOfType<SceneLoader>().ReloadScene();
                }
            }
        }
    }

    void GetGoals()
    {
        if(grid.world != null)
        {
            if(grid.world.levels[grid.level] != null)
            {
                levelGoals = grid.world.levels[grid.level].levelGoals;
            }
        }
    }

    void SetupGoals()
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
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
            if(levelGoals[i].numberCollected >= levelGoals[i].numberNeeded)
            {
                goalsCompleted++;
                currentGoals[i].SetPanelText(levelGoals[i].numberNeeded + "/" + levelGoals[i].numberNeeded);
            }
        }
        if(goalsCompleted >= levelGoals.Length)
        {
            Debug.Log("You win the level!");
            if(onLevelWon != null)
            {
                onLevelWon();
                onLevelWon -= FindObjectOfType<Level>().LevelTemplateComplete;
                okButton.SetActive(true);
                print("onWin delegate called");
            }
        }
    }

    public void CompareGoal(string goalToCompare)
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            if(levelGoals[i].matchTag == goalToCompare)
            {
                levelGoals[i].numberCollected++;
            }
        }
    }
}