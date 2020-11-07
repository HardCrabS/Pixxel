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
                        GameData.gameData.UnlockWorld(rewards[i].GetRewardId());
                        break;
                    }
                case LevelReward.BoostSlot:
                    {
                        GameData.gameData.UnlockSlotForBoost(System.Int32.Parse(rewards[i].GetRewardId()));
                        break;
                    }
                case LevelReward.Boost:
                    {
                        GameData.gameData.UnlockBoost(rewards[i].GetRewardId());
                        break;
                    }
                case LevelReward.Title:
                    {
                        GameData.gameData.UnlockTitle(rewards[i].GetRewardId());
                        break;
                    }
                case LevelReward.Banner:
                    {
                        GameData.gameData.UnlockBanner(rewards[i].GetRewardId());
                        break;
                    }
            }
        }
    }
}