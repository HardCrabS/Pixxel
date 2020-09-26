using UnityEngine;

[System.Serializable]
public class RewardTemplate
{
    public LevelReward reward;
    public string name;
    public Sprite rewardSprite;
    public int index;
}

[System.Serializable]
public class Reward 
{
    public RewardTemplate[] rewards;

	public void ApplyReward()
    {
        for (int i = 0; i < rewards.Length; i++)
        {
            switch (rewards[i].reward)
            {
                case LevelReward.World:
                    {
                        GameData.gameData.UnlockWorld(rewards[i].index);
                        break;
                    }
                case LevelReward.BoostSlot:
                    {
                        GameData.gameData.UnlockSlotForBoost(rewards[i].index);
                        break;
                    }
                case LevelReward.Boost:
                    {
                        GameData.gameData.UnlockBoost(rewards[i].index);
                        break;
                    }
                case LevelReward.Avatar:
                    {
                        GameData.gameData.UnlockAvatar(rewards[i].index);
                        break;
                    }
                case LevelReward.Title:
                    {
                        GameData.gameData.UnlockTitle(rewards[i].index);
                        break;
                    }
            }
        }
    }
}
