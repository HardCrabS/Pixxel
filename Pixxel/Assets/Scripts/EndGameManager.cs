using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameType
{
    Time,
    Moves
}

[System.Serializable]
public class EndGameRequirements
{
    public GameType gameType;
    public int counterValue;
}

public class EndGameManager : MonoBehaviour
{
    public EndGameRequirements requirements;
    [SerializeField] Text requirementText;
    [SerializeField] Text counterText;

    private int currentCounter;
    private float timerSeconds;
    GridA grid;


    void Start()
    {
        grid = FindObjectOfType<GridA>();
        SetGameType();
        SetRequirements();
    }

    void SetGameType()
    {
        if(grid.world != null)
        {
            if(grid.world.levels[grid.level] != null)
            {
                requirements = grid.world.levels[grid.level].endGameRequirements;
            }
        }
    }

    void SetRequirements()
    {
        currentCounter = requirements.counterValue;
        if (requirements.gameType == GameType.Moves)
        {
            requirementText.text = "Moves left: ";
        }
        else
        {
            timerSeconds = 1;
            requirementText.text = "Time left: ";
        }
        counterText.text = "" + currentCounter;
    }

    public void DecreaseCounterValue()
    {
        currentCounter--;
        counterText.text = "" + currentCounter;
        if (currentCounter <= 0)
        {
            Debug.Log("You lose!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (requirements.gameType == GameType.Time && currentCounter > 0)
        {
            timerSeconds -= Time.deltaTime;
            if (timerSeconds <= 0)
            {
                DecreaseCounterValue();
                timerSeconds = 1;
            }
        }
    }
}
