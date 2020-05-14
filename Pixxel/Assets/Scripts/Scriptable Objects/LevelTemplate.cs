using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "Level")]
public class LevelTemplate : ScriptableObject
{
    [Header("Grid Settings")]
    public int width;
    public int hight;
    public int offset;
    public TileType[] boardLayout;

    [Header("Prefabs")]
    public Box[] boxPrefabs;

    [Header("End Game Conditions")]
    public EndGameRequirements endGameRequirements;
    public LevelGoal[] levelGoals;

    [Header("Trinket Info")]
    public string trinketName;
    [TextArea(2,3)] public string requirementsExplained;
    public Sprite trinketSprite;

    [Header("Leaderboard Info")]
    public bool isLeaderboard = false;
    public int bombChance = 20;
}
