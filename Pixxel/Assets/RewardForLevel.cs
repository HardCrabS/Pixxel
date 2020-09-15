using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum LevelReward
{
    World,
    Trinket,
    Boost,
    BoostSlot,
    Title,
    Banner,
    Avatar,
    CardSet
}

public class RewardForLevel : MonoBehaviour
{
    [SerializeField] GameObject rewardEarned;
    [SerializeField] GameObject fireworksVFX;

    [SerializeField] Reward[] rewards;

    private void Start() // maybe change to Awake
    {
        int currPlayerLevel = GameData.gameData.saveData.currentLevel;
    }

    public void CheckForReward(int levelAchieved)
    {
        rewardEarned.GetComponent<Text>().text = "New Level!";
        rewards[levelAchieved - 1].ApplyReward();

        rewardEarned.GetComponent<Animation>().Play();
        LaunchFireworks();
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