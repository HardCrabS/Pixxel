﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorBonus : MonoBehaviour, IConcreteBonus
{
    [SerializeField] bool isGameField = true;
    [SerializeField] BlockTags tagToDestroyByBonus;
    [SerializeField] string uniqueAbility;
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
    public int GetBoostLevel()
    {
        return boostLevel;
    }
    public Sprite GetSpriteFromImage()
    {
        return GetComponent<Image>().sprite;
    }
    public void LevelUpBoost()
    {
        boostLevel++;
    }
    public int GetSpiteIndex()
    {
        return spriteIndex;
    }
    public string GetUniqueAbility(int level)
    {
        return uniqueAbility + "|";
    }
    public void SetBoostLevel(int lvl)
    {

    }
}