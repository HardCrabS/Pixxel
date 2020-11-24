using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Collection/Banner")]
public class Banner : RewardTemplate 
{
    [Header("Banner Specific")]
    [SerializeField] BannerName bannerName;
    [TextArea(2, 4)] [SerializeField] string description;
    [SerializeField] Sprite sprite;

    public string BannerName { get { return bannerName.ToString(); } }
    public string Description { get { return description; } }
    public Sprite Sprite { get { return sprite; } }

    public override Sprite GetRewardSprite()
    {
        return sprite;
    }
    public override string GetRewardId()
    {
        return bannerName.ToString();
    }
}

public enum BannerName
{
    LovelyOne,
    Friends,
    DarkWood,
}