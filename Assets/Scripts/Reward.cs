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
                        GameData.gameData.UnlockWorld(rewards[i].id);
                        break;
                    }
                case LevelReward.BoostSlot:
                    {
                        GameData.gameData.UnlockSlotForBoost(int.Parse(rewards[i].id));
                        break;
                    }
                case LevelReward.Boost:
                    {
                        GameData.gameData.UnlockBoost(rewards[i].id);
                        break;
                    }
                case LevelReward.Trinket:
                    {
                        GameData.gameData.UnlockTrinket(rewards[i].id);
                        break;
                    }
                case LevelReward.Title:
                    {
                        GameData.gameData.UnlockTitle(rewards[i].id);
                        break;
                    }
                case LevelReward.Banner:
                    {
                        GameData.gameData.UnlockBanner(rewards[i].id);
                        break;
                    }
                case LevelReward.CardSet:
                    {
                        GameData.gameData.UnlockCardSet(rewards[i].id);
                        break;
                    }
            }
        }
    }
}