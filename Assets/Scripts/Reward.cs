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
                        GameData.gameData.UnlockSlotForBoost(int.Parse(rewards[i].GetRewardId()));
                        break;
                    }
                case LevelReward.Boost:
                    {
                        GameData.gameData.UnlockBoost(rewards[i].GetRewardId());
                        break;
                    }
                case LevelReward.Trinket:
                    {
                        GameData.gameData.UnlockTrinket(rewards[i].GetRewardId());
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
                case LevelReward.CardSet:
                    {
                        GameData.gameData.UnlockCardSet(rewards[i].GetRewardId());
                        break;
                    }
            }
        }
    }
}