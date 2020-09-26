using UnityEngine;
using UnityEngine.Events;
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

    [SerializeField] GameObject rewardPanel;
    [SerializeField] Text rewardText;
    [SerializeField] Image rewardImage;

    [SerializeField] Reward[] rewards;

    public static RewardForLevel Instance;

    bool levelUped = false;

    void Awake()
    {
        Instance = this;
    }

    void Start() // maybe change to Awake
    {
        int currPlayerLevel = GameData.gameData.saveData.currentLevel;
    }

    public void CheckForReward(int levelAchieved)
    {
        rewardEarned.GetComponent<Text>().text = "New Level!";
        rewards[levelAchieved - 1].ApplyReward();

        SetRewardPanel(rewards[levelAchieved - 1].rewards);

        rewardEarned.GetComponent<Animation>().Play();
        LaunchFireworks();
    }

    public RewardTemplate GetReward(int level)
    {
        return rewards[level - 1].rewards[0];
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

    void SetRewardPanel(RewardTemplate[] rewards)
    {
        rewardText.text = "Your rewards:\n";
        for (int i = 0; i < rewards.Length; i++)
        {
            rewardText.text += rewards[i].reward + ": " + rewards[i].name + "\n\n";
            if (rewards[i].rewardSprite != null)
                rewardImage.sprite = rewards[i].rewardSprite;
        }
        levelUped = true;
    }
    public void CheckForLevelUpReward()
    {
        if(levelUped)
        {
            rewardPanel.SetActive(true);
        }
    }
}