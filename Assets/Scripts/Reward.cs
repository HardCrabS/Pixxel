﻿[System.Serializable]
public class Reward 
{
    public RewardTemplate[] rewards;

	public void ApplyReward()
    {
        for (int i = 0; i < rewards.Length; i++)
        {
            if (rewards[i] == null) continue;

            switch (rewards[i].reward)
            {
                case LevelReward.World:
                    {
                        GameData.gameData.UnlockWorld(rewards[i].id);
                        break;
                    }
                case LevelReward.BoostSlot:
                    {
                        //boost slots number starts from 1
                        int boostSlotIndex = int.Parse(rewards[i].id) - 1;
                        GameData.gameData.UnlockSlotForBoost(boostSlotIndex);
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