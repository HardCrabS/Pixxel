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
    [SerializeField] GameObject boostEarnedText;
    [SerializeField] GameObject fireworksVFX;
    [SerializeField] SceneLoader sceneLoader;
    Text boostText;

    [SerializeField] List<MyDictionaryEntry> inspectorBonusRewards;
    [SerializeField] List<MyDictionaryEntry> inspectorSlotRewards;
    [SerializeField] List<MyDictionaryEntry> inspectorWorldRewards;

    Dictionary<int, int> bonusRewards;
    Dictionary<int, int> slotRewards;
    Dictionary<int, int> worldRewards;

    private void Awake()
    {
        bonusRewards = new Dictionary<int, int>();
        slotRewards = new Dictionary<int, int>();
        worldRewards = new Dictionary<int, int>();
        foreach (MyDictionaryEntry entry in inspectorBonusRewards)
        {
            bonusRewards.Add(entry.level, entry.rewardIndex);
        }
        foreach (MyDictionaryEntry entry in inspectorSlotRewards)
        {
            slotRewards.Add(entry.level, entry.rewardIndex);
        }
        foreach (MyDictionaryEntry entry in inspectorWorldRewards)
        {
            worldRewards.Add(entry.level, entry.rewardIndex);
        }
    }
    void Start()
    {
        boostText = boostEarnedText.GetComponent<Text>();
        //boostEarnedText.GetComponent<Animation>().Play();
    }
    public void CheckForReward(int levelAchieved)
    {
        bool rewardPlayer = false;
        if (bonusRewards.ContainsKey(levelAchieved))
        {
            rewardPlayer = true;
            boostText.text = "New Boost!";
            GameData.gameData.UnlockBoost(bonusRewards[levelAchieved]);
        }
        else if (slotRewards.ContainsKey(levelAchieved))
        {
            rewardPlayer = true;
            boostText.text = "New Boost Slot!";
            GameData.gameData.UnlockSlotForBoost(slotRewards[levelAchieved]);
        }
        else if (worldRewards.ContainsKey(levelAchieved))
        {
            rewardPlayer = true;
            boostText.text = "New World!";
            GameData.gameData.UnlockWorld(worldRewards[levelAchieved]);
        }

        if(rewardPlayer)
        {
            boostEarnedText.GetComponent<Animation>().Play();
            LaunchFireworks();
            rewardPlayer = false;
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
}
