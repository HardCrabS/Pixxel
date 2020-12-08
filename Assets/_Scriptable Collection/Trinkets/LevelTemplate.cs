using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "Collection/Trinket")]
public class LevelTemplate : RewardTemplate
{
    [Header("Trinket Info")]
    public string trinketId;
    [TextArea(2, 3)] public string description;
    [TextArea(2, 3)] public string requirementsExplained;
    public Sprite trinketSprite;

    [Header("Trinket Goal")]
    public LevelGoal levelGoal;

    [Header("Grid Settings")]
    public int width;
    public int hight;
    public int offset;
    public TileType[] boardLayout;

    [Header("Leaderboard Info")]
    public int bombChance = 20;
    public override Sprite GetRewardSprite()
    {
        return trinketSprite;
    }
    public override string GetRewardId()
    {
        return trinketId;
    }
}