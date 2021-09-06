using UnityEngine;

[CreateAssetMenu(menuName = "Reward")]
public abstract class RewardTemplate : ScriptableObject 
{
    public LevelReward reward;
    public bool unlockedInShop = false;
    public string id;
    [TextArea(2, 3)] public string description;
    public int cost = 750;
    [TextArea(2, 3)] public string unlockRequirement;

    public virtual Sprite GetRewardSprite()
    {
        return null;
    }
}