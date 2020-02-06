using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorBonus : MonoBehaviour, IConcreteBonus
{
    [SerializeField] bool isGameField = true;
    [SerializeField] float timeForBonusReload = 5f;
    [SerializeField] tags tagToDestroyByBonus;
    [SerializeField] private int cost = 30;
    private string boostInfo = "Color Destroyer";
    private string description = "Destroys all blocks with concrete color.";
    private int boostLevel = 1;
    private int spriteIndex = 0;
    GridA grid;

    void Start()
    {
        if (isGameField)
        {
            grid = FindObjectOfType<GridA>();
        }
    }

    public void ExecuteBonus()
    {
        StartCoroutine(grid.DestroyAllSameColor(tagToDestroyByBonus.ToString()));
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
        return description;
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
    public int GetSpiteIndex()
    {
        return spriteIndex;
    }
    public void SetBoostLevel(int lvl)
    {
        boostLevel = lvl;
        switch (boostLevel)
        {
            case 1:
                {
                    timeForBonusReload = 10;
                    break;
                }
            case 2:
                {
                    timeForBonusReload = 5;
                    break;
                }
            case 3:
                {
                    timeForBonusReload = 3;
                    break;
                }
            default:
                {
                    timeForBonusReload = 3;
                    break;
                }
        }
    }
}