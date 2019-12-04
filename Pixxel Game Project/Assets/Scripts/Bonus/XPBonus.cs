using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XPBonus : MonoBehaviour, IConcreteBonus
{
    [SerializeField] bool isGameField = true;
    [SerializeField] float timeForBonusReload = 10f;
    [SerializeField] float procentForXP = 2;
    [SerializeField] float timeForBonusLast = 10f;
    [SerializeField] private int cost = 25;
    private string boostInfo = "XP Boost";
    private string description = "XP gained for each block increased by ";
    private int boostLevel = 1;
    bool needToResetXPprocent = false;
    float timer;
    GridA grid;

    void Start()
    {
        if (isGameField)
        {
            grid = FindObjectOfType<GridA>();
        }
    }

    void Update()
    {
        if (needToResetXPprocent)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                grid.SetXPpointsPerBoxByProcent(1/procentForXP);
                needToResetXPprocent = false;
            }
        }
    }

    public void ExecuteBonus()
    {
        grid.SetXPpointsPerBoxByProcent(procentForXP);
        needToResetXPprocent = true;
        timer = timeForBonusLast;
    }

    public Sprite GetSprite()
    {
        return GetComponent<SpriteRenderer>().sprite;
    }

    public float TimeToReload()
    {
        return timeForBonusReload;
    }

    public string GetBoostTitle()
    {
        return boostInfo;
    }
    public int GetBoostLevel()
    {
        return boostLevel;
    }
    public string GetBoostDescription()
    {
        return description + (procentForXP*100-100) + "%";
    }
    public Sprite GetSpriteFromImage()
    {
        return GetComponent<Image>().sprite;
    }
    public int GetBoostLevelUpCost()
    {
        return cost;
    }
    public void LevelUpBoost()
    {
        boostLevel++;
    }
    public void SetBoostLevel(int lvl)
    {
        boostLevel = lvl;
        switch (boostLevel)
        {
            case 1:
                timeForBonusReload = 5;
                break;
            case 2:
                timeForBonusReload = 4;
                break;
            case 3:
                timeForBonusReload = 2;
                break;
            default:
                timeForBonusReload = 2;
                break;
        }
    }
}
