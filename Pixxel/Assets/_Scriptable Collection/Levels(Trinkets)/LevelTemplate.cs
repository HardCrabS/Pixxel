using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "Reward/Trinket")]
public class LevelTemplate : RewardTemplate
{
    [Header("Grid Settings")]
    public int width;
    public int hight;
    public int offset;
    public TileType[] boardLayout;

    [Header("End Game Conditions")]
    public EndGameRequirements endGameRequirements;
    public LevelGoal[] levelGoals;

    [Header("Trinket Info")]
    [TextArea(2, 3)] public string description;
    [TextArea(2, 3)] public string requirementsExplained;
    public Sprite trinketSprite;
    public TrinketName trinketId;

    [Header("Leaderboard Info")]
    public bool isLeaderboard = false;
    public int bombChance = 20;
    public override Sprite GetRewardSprite()
    {
        return trinketSprite;
    }
    public override string GetRewardId()
    {
        return trinketId.ToString();
    }
}

public enum TrinketName
{
    PlasticCrown,
    JarOfJam,
    OldChest,
    SilverRing,
    DeliciousSoup,
    ThankYouNote,
    GDDocument,
}