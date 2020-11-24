using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Collection/Boost")]
public class Boost : RewardTemplate 
{
    [SerializeField] BoostName boostName;
    [TextArea(2, 4)] [SerializeField] string description;
    [SerializeField] int[] upgradeCosts;
    [SerializeField] float[] reloadSpeed;
    [SerializeField] Sprite[] upgradeSprites;
    [SerializeField] string boostTypeString;

    public string Title { get { return SplitCamelCase(boostName.ToString()); } }
    public string Description { get { return description; } }
    public int[] UpgradeCosts { get { return upgradeCosts; } }
    public float[] ReloadSpeed { get { return reloadSpeed; } }
    public Sprite[] UpgradeSprites { get { return upgradeSprites; } }
    public string BoostTypeString { get { return boostTypeString; } }

    public override string GetRewardId()
    {
        return boostName.ToString();
    }
    public override Sprite GetRewardSprite()
    {
        return upgradeSprites.Length > 1 ? upgradeSprites[2] : upgradeSprites[0];
    }
}

public enum BoostName
{
    ColorHater,
    Flamethrower,
    FrozenTower,
    FireFist,
    XPBooster,
    FlashField,
    Switcheroo,
    GoldRush
}