using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MyDictionaryEntry
{
    public int level;
    public int rewardIndex;
}
public class RewardForLevel : MonoBehaviour
{
    [SerializeField] GameObject rewardEarned;
    [SerializeField] GameObject fireworksVFX;

    Text rewardText;

    [SerializeField] List<MyDictionaryEntry> inspectorBonusRewards;
    [SerializeField] List<MyDictionaryEntry> inspectorSlotRewards;
    [SerializeField] List<MyDictionaryEntry> inspectorWorldRewards;

    Dictionary<int, int> bonusRewards;
    Dictionary<int, int> slotRewards;
    Dictionary<int, int> worldRewards;

    private void Start() // maybe change to Awake
    {
        int currPlayerLevel = GameData.gameData.saveData.currentLevel;

        bonusRewards = new Dictionary<int, int>();
        slotRewards = new Dictionary<int, int>();
        worldRewards = new Dictionary<int, int>();
        foreach (MyDictionaryEntry entry in inspectorBonusRewards)
        {
            if (currPlayerLevel < entry.level)
                bonusRewards.Add(entry.level, entry.rewardIndex);
        }
        foreach (MyDictionaryEntry entry in inspectorSlotRewards)
        {
            if (currPlayerLevel < entry.level)
                slotRewards.Add(entry.level, entry.rewardIndex);
        }
        foreach (MyDictionaryEntry entry in inspectorWorldRewards)
        {
            if (currPlayerLevel < entry.level)
                worldRewards.Add(entry.level, entry.rewardIndex);
        }

        if (rewardEarned != null)
            rewardText = rewardEarned.GetComponent<Text>();
    }
    /*void Start()
    {
        rewardText = rewardEarned.GetComponent<Text>();
        //boostEarnedText.GetComponent<Animation>().Play();
    }*/
    public void CheckForReward(int levelAchieved)
    {
        bool rewardPlayer = false;
        if (bonusRewards.ContainsKey(levelAchieved))
        {
            rewardPlayer = true;
            rewardText.text = "New Boost!";
            GameData.gameData.UnlockBoost(bonusRewards[levelAchieved]);
        }
        else if (slotRewards.ContainsKey(levelAchieved))
        {
            rewardPlayer = true;
            rewardText.text = "New Boost Slot!";
            GameData.gameData.UnlockSlotForBoost(slotRewards[levelAchieved]);
        }
        else if (worldRewards.ContainsKey(levelAchieved))
        {
            rewardPlayer = true;
            rewardText.text = "New World!";
            GameData.gameData.UnlockWorld(worldRewards[levelAchieved]);
        }

        if (rewardPlayer)
        {
            rewardEarned.GetComponent<Animation>().Play();
            LaunchFireworks();
        }
    }

    void LaunchFireworks()
    {
        if (fireworksVFX != null)
        {
            Vector2 bottomLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);
            Vector2 spawnPos = new Vector2(bottomLeft.x + Camera.main.orthographicSize / 2, bottomLeft.y);
            GameObject go = Instantiate(fireworksVFX, spawnPos, fireworksVFX.transform.rotation);
            Destroy(go, 8f);
        }
    }

    public int GetLevelsUntilReward(int nextLevel)
    {
        int levelsUntilReward = 1;
        for (int i = nextLevel; i < nextLevel + 3; i++) //looking 3 levels ahead
        {
            if (bonusRewards.ContainsKey(i) || slotRewards.ContainsKey(i) || worldRewards.ContainsKey(i))
            {
                return levelsUntilReward;
            }
            levelsUntilReward++;
        }
        return 0;
    }
}
