﻿using UnityEngine;

[CreateAssetMenu(menuName = "Collection/Title")]
public class Title : RewardTemplate 
{
    [Header("Title Specific")]
    public TitleString title;
    public override string GetRewardId()
    {
        return title.ToString();
    }
}
public enum TitleString
{
    Beginner,
    Collector,
    Master,
    Incredible,
}