using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boost")]
public class Boost : ScriptableObject 
{
    [SerializeField] string title;
    [TextArea(2, 4)] [SerializeField] string description;
    [SerializeField] int boostIndex;
    [SerializeField] int[] upgradeCosts;
    [SerializeField] float[] reloadSpeed;
    [SerializeField] Sprite[] upgradeSprites;
    [SerializeField] string boostTypeString;

    public string Title { get { return title; } }
    public string Description { get { return description; } }
    public int Index { get { return boostIndex; } }
    public int[] UpgradeCosts { get { return upgradeCosts; } }
    public float[] ReloadSpeed { get { return reloadSpeed; } }
    public Sprite[] UpgradeSprites { get { return upgradeSprites; } }
    public string BoostTypeString { get { return boostTypeString; } }
}