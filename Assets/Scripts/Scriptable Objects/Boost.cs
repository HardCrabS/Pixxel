using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Collection/Boost")]
public class Boost : RewardTemplate 
{
    [SerializeField] Vector2[] costAndReload = new Vector2[10];
    [SerializeField] Sprite[] upgradeSprites;
    [SerializeField] string boostTypeString;
    public int GetUpgradeCost(int boostLevel) => boostLevel < costAndReload.Length ? (int)costAndReload[boostLevel].x : -1;
    public int GetReloadSpeed(int boostLevel) => boostLevel - 1 < costAndReload.Length ? (int)costAndReload[boostLevel - 1].y : -1;
    public Sprite[] UpgradeSprites { get { return upgradeSprites; } }
    public string BoostTypeString { get { return boostTypeString; } }

    public override Sprite GetRewardSprite()
    {
        return upgradeSprites.Length > 1 ? upgradeSprites[2] : upgradeSprites[0];
    }
}